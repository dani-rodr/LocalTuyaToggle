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

        private readonly StatusRequest _statusRequest;
        private readonly CommandRequest _commandRequest;

        public DeviceController()
        {
            _statusRequest = new StatusRequest(_clientId, _secret, _deviceId);
            _commandRequest = new CommandRequest(_clientId, _secret, _deviceId);
        }

        public async Task<bool> IsActiveAsync()
        {
            await RetrieveToken();
            return await _statusRequest.IsOnAsync(_token);
        }

        public async Task<bool> TurnOnAsync()
        {
            await RetrieveToken();
            return await _commandRequest.TurnOnAsync(_token);
        }

        public async Task<bool> TurnOffAsync()
        {
            await RetrieveToken();
            return await _commandRequest.TurnOffAsync(_token);
        }

        private async Task RetrieveToken()
        {
            if (!string.IsNullOrEmpty(_token))
            {
                return;
            }

            if (await IsTokenExpired())
            {
                await RequestToken();
            }

            _token = await SecureStorage.GetAsync(_accessTokenKey);
        }

        private async Task RequestToken()
        {
            var tokenRequest = new TokenRequest(_clientId, _secret);
            var tokenResponse = await tokenRequest.GetToken();
            var result = tokenResponse.result;
            var expiration = Convert.ToInt64(tokenResponse.t) + Convert.ToInt64(result.expire_time) * 1000;
            _token = result.access_token;
            await SecureStorage.SetAsync(_accessTokenKey, _token);
            await SecureStorage.SetAsync(_expKey, expiration.ToString());
        }

        private async Task<bool> IsTokenExpired()
        {
            var expirationString = await SecureStorage.GetAsync(_expKey);
            var expiration = Convert.ToInt64(expirationString);
            return (Helper.TimeStamp > expiration);
        }
    }
}
