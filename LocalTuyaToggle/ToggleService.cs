﻿
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
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
            var tokenRequest = new TokenRequest(_clientId, _secret);
            _token = await tokenRequest.GetToken();
            await SetTileCurrentState();
        }

        private async Task SetTileCurrentState()
        {
            var tile = QsTile;
            var deviceStatusRequest = new StatusRequest(_clientId, _secret, _token, _deviceId);
            var isOn = await deviceStatusRequest.IsOnAsync();
            if (isOn && tile.State != TileState.Active)
            {
                tile.State = TileState.Active;
            }
            if (!isOn && tile.State != TileState.Inactive)
            {
                tile.State = TileState.Inactive;
            }
            tile.UpdateTile();
        }

        public async override void OnClick()
        {
            base.OnClick();
            var tile = QsTile;
            var serviceRequest = new CommandRequest(_clientId, _secret, _token, _deviceId);
            if (tile.State == TileState.Active)
            {
                var success = await serviceRequest.TurnOffAsnyc();
                if (success)
                {
                    tile.State = TileState.Inactive;
                }
            }
            else if (tile.State == TileState.Inactive)
            {
                var success = await serviceRequest.TurnOnAsync();
                if (success)
                {
                    tile.State = TileState.Active;
                }
            }
            tile.UpdateTile();
        }

        private async Task<bool> IsDeviceOn()
        {
            var deviceStatusRequest = new StatusRequest(_clientId, _secret, _token, _deviceId);
            return await deviceStatusRequest.IsOnAsync();
        }
    }

}
