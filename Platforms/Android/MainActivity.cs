using Android.App;
using Android.Content.PM;
using Android.OS;

namespace DailyBibleVerseApp;

// CS0117 fix: LocalNotificationCenter.CreateNotificationChannel() was removed
// in newer versions of Plugin.LocalNotification (v11+).
// The channel is now created automatically by the plugin — no manual call needed.
[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges =
        ConfigChanges.ScreenSize | ConfigChanges.Orientation |
        ConfigChanges.UiMode | ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        // Plugin.LocalNotification v11+ auto-creates the notification channel.
        // No manual call required here.
    }
}
