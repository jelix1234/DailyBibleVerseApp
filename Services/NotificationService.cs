// Alias our own interface to avoid collision with Plugin.LocalNotification.INotificationService
using IAppNotificationService = DailyBibleVerseApp.Interfaces.INotificationService;
using Plugin.LocalNotification;

namespace DailyBibleVerseApp.Services
{
    // NotificationService: concrete IAppNotificationService.
    // Schedules a repeating daily push notification at the user's chosen time.
    // The alias above resolves CS0104 — both this app and Plugin.LocalNotification
    // define INotificationService, so we qualify ours explicitly.
    public class NotificationService : IAppNotificationService
    {
        public async Task<bool> RequestPermissionAsync() =>
            await LocalNotificationCenter.Current.RequestNotificationPermission();

        public async Task ScheduleDailyReminderAsync(TimeSpan time)
        {
            await CancelAllAsync();
            var notifyAt = DateTime.Today.Add(time) < DateTime.Now
                ? DateTime.Today.AddDays(1).Add(time)
                : DateTime.Today.Add(time);

            var request = new NotificationRequest
            {
                NotificationId = 2001,
                Title = "Daily Bible Verse",
                Description = "Your daily verse is waiting. Open the app to read and reflect.",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyAt,
                    RepeatType = NotificationRepeat.Daily
                }
            };
            await LocalNotificationCenter.Current.Show(request);
        }

        public Task CancelAllAsync()
        {
            LocalNotificationCenter.Current.CancelAll();
            return Task.CompletedTask;
        }
    }
}
