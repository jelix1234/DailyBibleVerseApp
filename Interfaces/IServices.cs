using DailyBibleVerseApp.Models;

namespace DailyBibleVerseApp.Interfaces
{
    // ── IDatabaseService ───────────────────────────────────────────────────────
    public interface IDatabaseService
    {
        Task InitialiseAsync();

        // User CRUD
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByIdAsync(int id);
        Task<int> SaveUserAsync(User user);
        Task<bool> UsernameExistsAsync(string username);

        // FavoriteVerse CRUD
        Task<List<FavoriteVerse>> GetFavouritesAsync();
        Task<FavoriteVerse?> GetFavouriteByIdAsync(int id);
        Task<int> SaveFavouriteAsync(FavoriteVerse verse);
        Task<int> DeleteFavouriteAsync(int id);
        Task<int> ClearFavouritesAsync();

        // Notes CRUD
        Task<List<Note>> GetNotesAsync();
        Task<Note?> GetNoteByIdAsync(int id);
        Task<int> SaveNoteAsync(Note note);
        Task<int> DeleteNoteAsync(int id);
    }

    // ── IAuthService ───────────────────────────────────────────────────────────
    public interface IAuthService
    {
        User? CurrentUser { get; }
        bool IsLoggedIn { get; }
        Task<(bool Success, string Message)> LoginAsync(string username, string password);
        Task<(bool Success, string Message)> RegisterAsync(string username, string password);
        Task LogoutAsync();
        Task<bool> TryAutoLoginAsync();
        Task UpdateStreakAsync();
        bool IsUserLoggedIn();
    }

    // ── IBibleVerseService ─────────────────────────────────────────────────────
    public interface IBibleVerseService
    {
        Task<Models.BibleVerse> GetVerseAsync(string book, int chapter, int verse, string translation = "kjv");
        Task<Models.BibleVerse> GetDailyVerseAsync(string translation = "kjv");
        Task<Models.BibleVerse> GetRandomVerseAsync(string translation = "kjv");
    }

    // ── INotificationService ───────────────────────────────────────────────────
    // NOTE: Named to match our alias in NotificationService.cs.
    // Plugin.LocalNotification also defines INotificationService — we resolve the
    // CS0104 ambiguous-reference error by using a using-alias in the concrete class:
    //   using IAppNotificationService = DailyBibleVerseApp.Interfaces.INotificationService;
    public interface INotificationService
    {
        Task<bool> RequestPermissionAsync();
        Task ScheduleDailyReminderAsync(TimeSpan time);
        Task CancelAllAsync();
    }
}
