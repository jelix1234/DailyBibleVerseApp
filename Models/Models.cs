using SQLite;

namespace DailyBibleVerseApp.Models
{
    // ── BibleVerse ─────────────────────────────────────────────────────────────
    // Represents a single Bible verse fetched from the external API.
    public class BibleVerse
    {
        public string Reference { get; set; } = "Unknown Reference";
        public string Text { get; set; } = "No verse available.";
        public string Translation { get; set; } = "kjv";
    }

    // ── BibleApiResponse ───────────────────────────────────────────────────────
    // Maps the JSON structure returned by bible-api.com.
    // Used by System.Text.Json for deserialisation.
    public class BibleApiResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("reference")]
        public string? Reference { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("text")]
        public string? Text { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("translation_name")]
        public string? TranslationName { get; set; }
    }

    // ── FavoriteVerse ──────────────────────────────────────────────────────────
    // Stored in SQLite. Represents a verse the user has saved.
    public class FavoriteVerse
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Book { get; set; } = "";
        public int Chapter { get; set; }
        public int Verse { get; set; }
        public string VerseText { get; set; } = "";
        public string Reference { get; set; } = "";
        public DateTime SavedAt { get; set; } = DateTime.Now;
    }

    // ── Note ───────────────────────────────────────────────────────────────────
    // Stored in SQLite. Full CRUD is supported via IDatabaseService.
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? LinkedVerse { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    // ── User ───────────────────────────────────────────────────────────────────
    // Stored in SQLite. Passwords are hashed (SHA-256) before saving.
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int StreakDays { get; set; } = 0;
        public int VersesRead { get; set; } = 0;
        public DateTime LastVisit { get; set; } = DateTime.MinValue;
        public bool NotificationsEnabled { get; set; } = true;
        public TimeSpan NotificationTime { get; set; } = new TimeSpan(8, 0, 0);
    }
}
