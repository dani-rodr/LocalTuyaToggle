using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace LocalTuyaToggle
{
    public class DeviceController
    {
        private const string _clientId = "mgejajtfb8jp6120o6r4";
        private const string _secret = "485d86f159c24281b77bec1a22e759bc";
        private const string _deviceId = "82315003c44f33f81519";
        private const string _expKey = "expiration";
        private const string _accessTokenKey = "access_token";
        private string _token = string.Empty;
        private long _expiration = 0;

        private readonly StatusRequest _statusRequest;
        private readonly CommandRequest _commandRequest;
        private readonly TokenRequest _tokenRequest;

        public DeviceController()
        {
            _statusRequest = new StatusRequest(_clientId, _secret, _deviceId);
            _commandRequest = new CommandRequest(_clientId, _secret, _deviceId);
            _tokenRequest = new TokenRequest(_clientId, _secret);
        }

        public async Task<bool> IsActiveAsync()
        {
            var token = await RetrieveTokenAsync();
            return await _statusRequest.IsActiveAsync(token);
        }

        public async Task<bool> TurnOnAsync()
        {
            var token = await RetrieveTokenAsync();
            return await _commandRequest.TurnOnAsync(token);
        }

        public async Task<bool> TurnOffAsync()
        {
            var token = await RetrieveTokenAsync();
            return await _commandRequest.TurnOffAsync(token);
        }

        private async Task<string> RetrieveTokenAsync()
        {
            var shouldGetFromStorage = _expiration == 0 || string.IsNullOrEmpty(_token);
            if (shouldGetFromStorage)
            {
                _expiration = Convert.ToInt64(await SecureStorage.GetAsync(_expKey));
                _token = await SecureStorage.GetAsync(_accessTokenKey);
            }
            if (_expiration > Helper.TimeStamp)
            {
                return _token;
            }

            return await RequestTokenAsync();
        }

        private async Task<string> RequestTokenAsync()
        {
            var tokenResponse = await _tokenRequest.GetToken();
            var result = tokenResponse.result;

            var _expiration = Convert.ToInt64(tokenResponse.t) + Convert.ToInt64(result.expire_time) * 1000;
            var _token = result.access_token;

            await SecureStorage.SetAsync(_accessTokenKey, _token);
            await SecureStorage.SetAsync(_expKey, _expiration.ToString());
            return _token;
        }
    }
}
