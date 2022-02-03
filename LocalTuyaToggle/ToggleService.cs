
using System;

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
            Console.WriteLine("Start Listening");
            var tokenRequest = new TokenRequest(_clientId, _secret);
            var response = await tokenRequest.RequestToken();
            if (response.success)
            {
                _token = response.result.access_token;
                Console.WriteLine($"Token is {response.result.access_token}");
            }
            else
            {
                Console.WriteLine($"{response.msg}");
            }

            var deviceStatusRequest = new ServiceRequestStatus(_clientId, _secret, _token, _deviceId);
            await deviceStatusRequest.RequestService();

        }
        public async override void OnClick()
        {
            base.OnClick();
            var tile = QsTile;
            var serviceRequest = new ServiceRequest(_clientId, _secret, _token, _deviceId);
            if (tile.State == TileState.Active)
            {
                await serviceRequest.RequestService("false");
                tile.State = TileState.Inactive;
            }
            else
            {
                await serviceRequest.RequestService("true");
                tile.State = TileState.Active;
            }
            tile.UpdateTile();
        }
    }

}
