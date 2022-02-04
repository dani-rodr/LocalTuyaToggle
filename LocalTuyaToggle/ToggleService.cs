
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Service.QuickSettings;

namespace LocalTuyaToggle
{
    [Service(Name = "com.LocalTuyaToggle.ToggleService",
             Permission = Android.Manifest.Permission.BindQuickSettingsTile,
             Label = "Lights",
             Icon = "@mipmap/ic_launcher")]
    [IntentFilter(new String[] { "com.yourname.ToggleService" })]
    [IntentFilter(new[] { ActionQsTile })]
    public class ToggleService : TileService
    {
        private readonly string _clientId = "mgejajtfb8jp6120o6r4";
        private readonly string _secret = "485d86f159c24281b77bec1a22e759bc";
        private readonly string _deviceId = "82315003c44f33f81519";
        private string _token = string.Empty;
        //Called each time tile is visible
        public async override void OnStartListening()
        {
            base.OnStartListening();
            var tile = QsTile;
            if (!string.IsNullOrEmpty(_token))
            {
                //await SetTileState();
                return;
            }
            //tile.State = TileState.Unavailable;
            //tile.UpdateTile();
            var tokenRequest = new TokenRequest(_clientId, _secret);
            var response = await tokenRequest.RequestToken();
            if (!response.success)
            {   
                Console.WriteLine($"{response.msg}");
                return;
            }
            _token = response.result.access_token;
            await SetTileState();
        }

        public async override void OnStopListening()
        {
            base.OnStopListening();
            await SetTileState();
        }

        public async override void OnClick()
        {
            base.OnClick();
            var tile = QsTile;
            var serviceRequest = new ServiceRequest(_clientId, _secret, _token, _deviceId);
            if (tile.State == TileState.Active)
            {
                var response = await serviceRequest.RequestCommand("false");
                if (response.success)
                {
                    tile.State = TileState.Inactive;
                }
            }
            else
            {
                var response = await serviceRequest.RequestCommand("true");
                if (response.success)
                {
                    tile.State = TileState.Active;
                }
            }
            tile.UpdateTile();
        }

        private async Task SetTileState()
        {
            var tile = QsTile;
            var isDeviceOn = await IsDeviceOn();
            tile.State = (isDeviceOn) ? TileState.Active : TileState.Inactive;
            tile.UpdateTile();
        }

        private async Task<bool> IsDeviceOn()
        {
            var deviceStatusRequest = new ServiceRequestStatus(_clientId, _secret, _token, _deviceId);
            var status = await deviceStatusRequest.RequestDeviceStatus();
            if (!status.success)
            {
                return false;
            }
            var isOn = (JsonElement) status.result[0].value;
            return isOn.GetBoolean();
        }
    }

}
