
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
        //Called each time tile is visible
        public override void OnStartListening()
        {
            base.OnStartListening();
            Console.WriteLine("Start Listening");
        }
        public override void OnClick()
        {
            base.OnClick();
            Console.WriteLine("Clicked");
        }
    }

}
