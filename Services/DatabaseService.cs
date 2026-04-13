using DailyBibleVerseApp.Interfaces;
using DailyBibleVerseApp.Models;
using SQLite;

namespace DailyBibleVerseApp.Services
{
    // DatabaseService: concrete IDatabaseService using SQLite.
    // Singleton in DI container. Demonstrates Repository Pattern + all CRUD ops.
    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection? _db;
        private readonly string _dbPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "dailybible.db3");
        }

        public async Task InitialiseAsync()
        {
            if (_db is not null) return;
            _db = new SQLiteAsyncConnection(_dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            await _db.CreateTableAsync<User>();
            await _db.CreateTableAsync<FavoriteVerse>();
            await _db.CreateTableAsync<Note>();
        }

        private SQLiteAsyncConnection Db =>
            _db ?? throw new InvalidOperationException("Call InitialiseAsync() first.");

        // User CRUD
        public Task<User?> GetUserByUsernameAsync(string username) =>
            Db.Table<User>().Where(u => u.Username == username.ToLower()).FirstOrDefaultAsync()!;
        public Task<User?> GetUserByIdAsync(int id) => Db.FindAsync<User>(id)!;
        public Task<int> SaveUserAsync(User user) =>
            user.Id == 0 ? Db.InsertAsync(user) : Db.UpdateAsync(user);
        public async Task<bool> UsernameExistsAsync(string username) =>
            await Db.Table<User>().Where(u => u.Username == username.ToLower()).CountAsync() > 0;

        // FavoriteVerse CRUD
        public Task<List<FavoriteVerse>> GetFavouritesAsync() =>
            Db.Table<FavoriteVerse>().OrderByDescending(f => f.SavedAt).ToListAsync();
        public Task<FavoriteVerse?> GetFavouriteByIdAsync(int id) => Db.FindAsync<FavoriteVerse>(id)!;
        public Task<int> SaveFavouriteAsync(FavoriteVerse verse) =>
            verse.Id == 0 ? Db.InsertAsync(verse) : Db.UpdateAsync(verse);
        public Task<int> DeleteFavouriteAsync(int id) => Db.DeleteAsync<FavoriteVerse>(id);
        public Task<int> ClearFavouritesAsync() => Db.DeleteAllAsync<FavoriteVerse>();

        // Notes CRUD
        public Task<List<Note>> GetNotesAsync() =>
            Db.Table<Note>().OrderByDescending(n => n.UpdatedAt).ToListAsync();
        public Task<Note?> GetNoteByIdAsync(int id) => Db.FindAsync<Note>(id)!;
        public Task<int> SaveNoteAsync(Note note)
        {
            note.UpdatedAt = DateTime.Now;
            return note.Id == 0 ? Db.InsertAsync(note) : Db.UpdateAsync(note);
        }
        public Task<int> DeleteNoteAsync(int id) => Db.DeleteAsync<Note>(id);
    }
}
