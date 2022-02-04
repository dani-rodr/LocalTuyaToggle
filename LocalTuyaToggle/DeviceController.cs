using System;
using System.Threading.Tasks;

namespace LocalTuyaToggle
{
    public class DeviceController
    {
        private const string _clientId = "mgejajtfb8jp6120o6r4";
        private const string _secret = "485d86f159c24281b77bec1a22e759bc";
        private const string _deviceId = "82315003c44f33f81519";
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
            await RequestToken();
            return await _statusRequest.IsOnAsync(_token);
        }

        public async Task<bool> TurnOnAsync()
        {
            await RequestToken();
            return await _commandRequest.TurnOnAsync(_token);
        }

        public async Task<bool> TurnOffAsync()
        {
            await RequestToken();
            return await _commandRequest.TurnOffAsync(_token);
        }

        private async Task RequestToken()
        {
            if (!string.IsNullOrEmpty(_token))
            {
                return;
            }
            var tokenRequest = new TokenRequest(_clientId, _secret);
            _token = await tokenRequest.GetToken();
            Console.WriteLine("Token Request");
        }
    }
}
