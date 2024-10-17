using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Fingerprint;

namespace Asis_Batia;

//[Activity( Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
public class MainActivity : MauiAppCompatActivity {
    protected override void OnCreate(Bundle savedInstanceState) {
        base.OnCreate(savedInstanceState);
        CrossFingerprint.SetCurrentActivityResolver(() => this);
    }

}
