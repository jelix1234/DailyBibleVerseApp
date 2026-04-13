using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyBibleVerseApp.Interfaces;

namespace DailyBibleVerseApp.ViewModels
{
    // SettingsViewModel: profile display, notification scheduling, logout.
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly IAuthService _auth;
        private readonly INotificationService _notif;
        private readonly IDatabaseService _db;

        [ObservableProperty] private string _username = string.Empty;
        [ObservableProperty] private int _streakDays;
        [ObservableProperty] private int _versesRead;
        [ObservableProperty] private bool _notificationsEnabled;
        [ObservableProperty] private TimeSpan _notificationTime = new(8, 0, 0);

        public SettingsViewModel(IAuthService auth, INotificationService notif, IDatabaseService db)
        {
            _auth = auth;
            _notif = notif;
            _db = db;
            Title = "Settings";
        }

        [RelayCommand]
        public void Load()
        {
            var u = _auth.CurrentUser;
            if (u is null) return;
            Username = u.Username;
            StreakDays = u.StreakDays;
            VersesRead = u.VersesRead;
            NotificationsEnabled = u.NotificationsEnabled;
            NotificationTime = u.NotificationTime;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (_auth.CurrentUser is null) return;
            IsBusy = true;
            try
            {
                _auth.CurrentUser.NotificationsEnabled = NotificationsEnabled;
                _auth.CurrentUser.NotificationTime = NotificationTime;
                await _db.SaveUserAsync(_auth.CurrentUser);

                if (NotificationsEnabled)
                {
                    bool granted = await _notif.RequestPermissionAsync();
                    if (granted)
                        await _notif.ScheduleDailyReminderAsync(NotificationTime);
                    else
                        await Shell.Current.DisplayAlert("Permission Denied",
                            "Enable notifications in device settings.", "OK");
                }
                else
                {
                    await _notif.CancelAllAsync();
                }
                await Shell.Current.DisplayAlert("Saved", "Settings updated.", "OK");
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool ok = await Shell.Current.DisplayAlert("Logout", "Are you sure?", "Logout", "Cancel");
            if (!ok) return;
            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
