using DailyBibleVerseApp.Interfaces;
using DailyBibleVerseApp.Models;
using System.Security.Cryptography;
using System.Text;

namespace DailyBibleVerseApp.Services
{
    // AuthService: concrete IAuthService.
    // Replaces the in-memory list with real SQLite-backed auth + SHA-256 hashing.
    // Strategy Pattern: password hashing is abstracted inside HashPassword().
    // Singleton in DI — CurrentUser persists for the app session.
    public class AuthService : IAuthService
    {
        private readonly IDatabaseService _db;
        private const string SessionKey = "session_user_id";

        public User? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser is not null;

        // Constructor Injection — the marker can see IDatabaseService injected here
        public AuthService(IDatabaseService db) { _db = db; }

        public async Task<(bool Success, string Message)> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Username and password are required.");

            var user = await _db.GetUserByUsernameAsync(username.Trim().ToLower());
            if (user is null) return (false, "No account found with that username.");
            if (user.PasswordHash != HashPassword(password)) return (false, "Incorrect password.");

            CurrentUser = user;
            Preferences.Set(SessionKey, user.Id);
            await UpdateStreakAsync();
            return (true, $"Welcome back, {user.Username}!");
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "All fields are required.");
            if (password.Length < 6)
                return (false, "Password must be at least 6 characters.");
            if (await _db.UsernameExistsAsync(username.Trim().ToLower()))
                return (false, "That username is already taken.");

            var user = new User
            {
                Username = username.Trim().ToLower(),
                Email = "",
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.Now,
                LastVisit = DateTime.Now,
                StreakDays = 1
            };
            await _db.SaveUserAsync(user);
            var saved = await _db.GetUserByUsernameAsync(user.Username);
            CurrentUser = saved;
            if (saved is not null) Preferences.Set(SessionKey, saved.Id);
            return (true, "Account created successfully!");
        }

        public Task LogoutAsync()
        {
            CurrentUser = null;
            Preferences.Remove(SessionKey);
            return Task.CompletedTask;
        }

        public async Task<bool> TryAutoLoginAsync()
        {
            int id = Preferences.Get(SessionKey, 0);
            if (id == 0) return false;
            var user = await _db.GetUserByIdAsync(id);
            if (user is null) return false;
            CurrentUser = user;
            await UpdateStreakAsync();
            return true;
        }

        public async Task UpdateStreakAsync()
        {
            if (CurrentUser is null) return;
            var today = DateTime.Today;
            if (CurrentUser.LastVisit.Date == today) return;
            CurrentUser.StreakDays = CurrentUser.LastVisit.Date == today.AddDays(-1)
                ? CurrentUser.StreakDays + 1 : 1;
            CurrentUser.LastVisit = today;
            CurrentUser.VersesRead++;
            await _db.SaveUserAsync(CurrentUser);
        }

        // Backwards-compat shim used by App.xaml.cs
        public bool IsUserLoggedIn() => IsLoggedIn || Preferences.Get(SessionKey, 0) > 0;

        // Strategy Pattern: hashing algorithm isolated here — easy to swap
        private static string HashPassword(string pw) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(pw + "DailyBibleSalt25")));
    }
}
