// StorageService is now a thin compatibility shim.
// Real storage goes through IDatabaseService (DatabaseService).
// Kept so existing pages that reference it still compile during refactor.
using DailyBibleVerseApp.Interfaces;
using DailyBibleVerseApp.Models;

namespace DailyBibleVerseApp.Services
{
    [Obsolete("Use IDatabaseService directly via constructor injection instead.")]
    public class StorageService
    {
        private readonly IDatabaseService _db;
        public StorageService(IDatabaseService db) { _db = db; }

        public Task SaveFavorite(FavoriteVerse verse) => _db.SaveFavouriteAsync(verse).ContinueWith(_ => { });
        public Task<List<FavoriteVerse>> GetFavoriteVersesAsync() => _db.GetFavouritesAsync();
        public Task ClearFavoritesAsync() => _db.ClearFavouritesAsync().ContinueWith(_ => { });

        public Task SaveUserNote(string text) =>
            _db.SaveNoteAsync(new Note { Title = text.Length > 30 ? text[..30] : text, Text = text });
        public async Task<List<string>> GetUserNotes() =>
            (await _db.GetNotesAsync()).Select(n => n.Text).ToList();
        public async Task DeleteNoteAsync(string noteText)
        {
            var notes = await _db.GetNotesAsync();
            var match = notes.FirstOrDefault(n => n.Text == noteText);
            if (match is not null) await _db.DeleteNoteAsync(match.Id);
        }
        public Task ClearNotesAsync() => Task.CompletedTask;
        public Task<List<string>> GetUserNotesAsync() => GetUserNotes();
    }
}
