using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Service.QuickSettings;

namespace LocalTuyaToggle
{
    [Service(Name = "com.LocalTuyaToggle.ToggleService",
             Permission = Android.Manifest.Permission.BindQuickSettingsTile,
             Label = "Lights",
             Icon = "@mipmap/ic_launcher")]
    [IntentFilter(new[] { ActionQsTile })]
    public class ToggleService : TileService
    {
        private DeviceController _deviceController = new DeviceController();

        public async override void OnClick()
        {
            base.OnClick();
            var state = QsTile.State;
            if (state == TileState.Active)
            {
                await TurnOffDeviceAsync();
                return;
            }
            if (state == TileState.Inactive)
            {
                await TurnOnDeviceAsync();
                return;
            }
        }

        private async Task TurnOffDeviceAsync()
        {
            SetSubtitle("Turning off");
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
            SetSubtitle("Turning on");
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

        private void SetSubtitle(string str)
        {
            var tile = QsTile;
            tile.Subtitle = str;
            tile.UpdateTile();
        }
    }

}
