
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
        private DeviceController _deviceController = new DeviceController();
        //Called each time tile is visible
        public async override void OnStartListening()
        {
            base.OnStartListening();
            var state = QsTile.State;
            var isDeviceActive = await _deviceController.IsActiveAsync();

            if (isDeviceActive && state != TileState.Active)
            {
                ActivateTile();
            }
            else if (!isDeviceActive && state != TileState.Inactive)
            {
                DeactivateTile();
            }
        }
        public async override void OnClick()
        {
            base.OnClick();
            var state = QsTile.State;
            if (state == TileState.Active && await _deviceController.TurnOffAsync())
            {
                DeactivateTile();
            }
            else if (state == TileState.Inactive && await _deviceController.TurnOnAsync())
            {
                ActivateTile();
            }
        }

        private void ActivateTile()
        {
            var tile = QsTile;
            tile.State = TileState.Active;
            tile.UpdateTile();
        }

        private void DeactivateTile()
        {
            var tile = QsTile;
            tile.State = TileState.Inactive;
            tile.UpdateTile();
        }
    }

}
