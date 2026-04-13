namespace DailyBibleVerseApp.Helpers
{
    // DateHelper: pure static utility — no dependencies, no side effects.
    // Separation of concerns: date/time logic lives here, not in ViewModels.
    public static class DateHelper
    {
        public static string GetGreeting()
        {
            return DateTime.Now.Hour switch
            {
                < 12 => "Good morning",
                < 17 => "Good afternoon",
                _ => "Good evening"
            };
        }

        public static string FormatRelativeDate(DateTime date)
        {
            var diff = DateTime.Now - date;
            return diff.TotalSeconds < 60 ? "Just now"
                : diff.TotalMinutes < 60 ? $"{(int)diff.TotalMinutes}m ago"
                : diff.TotalHours < 24 ? $"{(int)diff.TotalHours}h ago"
                : diff.TotalDays < 7 ? $"{(int)diff.TotalDays}d ago"
                : date.ToString("MMM d, yyyy");
        }

        public static string GetStreakMessage(int days) => days switch
        {
            0 => "Start your streak today",
            1 => "1 day — great start!",
            <= 6 => $"{days} days — keep going!",
            <= 29 => $"{days} days — you're on fire!",
            _ => $"{days} days — outstanding!"
        };
    }
}
