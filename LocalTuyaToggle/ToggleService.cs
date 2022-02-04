using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Service.QuickSettings;

namespace LocalTuyaToggle
{
    [Service(Name = "com.LocalTuyaToggle.ToggleService",
             Permission = Android.Manifest.Permission.BindQuickSettingsTile,
             Label = "Lights",
             Icon = "@mipmap/light_toggle")]
    [IntentFilter(new[] { ActionQsTile })]
    public class ToggleService : TileService
    {
        private DeviceController _deviceController = new DeviceController();
        //Called each time tile is visible
        public async override void OnStartListening()
        {
            base.OnStartListening();
            var state = QsTile.State;
            var isDeviceActive = await IsActiveAsync();

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
            if (state == TileState.Active)
            {
                await TurnOffDeviceAsync();
                return;
            }
            else if (state == TileState.Inactive)
            {
                await TurnOnDeviceAsync();
                return;
            }
        }

        private async Task TurnOffDeviceAsync()
        {
            SetSubtitleTurningOff();
            var success = await _deviceController.TurnOffAsync();
            if (success)
            {
                DeactivateTile();
            }
            else
            {
                ActivateTile();
            }
        }

        private async Task TurnOnDeviceAsync()
        {
            SetSubtitleTurningOn();
            var success = await _deviceController.TurnOnAsync();
            if (success)
            {
                ActivateTile();
            }
            else
            {
                DeactivateTile();
            }
        }

        private void ActivateTile()
        {
            var tile = QsTile;
            tile.State = TileState.Active;
            tile.Subtitle = "On";
            tile.UpdateTile();
        }

        private void DeactivateTile()
        {
            var tile = QsTile;
            tile.State = TileState.Inactive;
            tile.Subtitle = "Off";
            tile.UpdateTile();
        }

        private void SetSubtitle(string str = "")
        {
            var tile = QsTile;
            tile.Subtitle = str;
            tile.UpdateTile();
        }

        private void SetSubtitleTurningOn()
        {
            SetSubtitle("Turning on");
        }

        private void SetSubtitleTurningOff()
        {
            SetSubtitle("Turning off");
        }

        private async Task<bool> IsActiveAsync()
        {
            SetSubtitle("Connecting..");
            var isActive = await _deviceController.IsActiveAsync();
            var subtitle = (isActive) ? "On" : "Off";
            SetSubtitle(subtitle);
            return isActive;
        }
    }

}
