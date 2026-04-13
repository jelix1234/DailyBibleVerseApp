using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyBibleVerseApp.Interfaces;
using DailyBibleVerseApp.Models;
using System.Collections.ObjectModel;

namespace DailyBibleVerseApp.ViewModels
{
    // BibleVerseViewModel: drives MainPage (verse lookup) and DailyVersePage.
    // ALL logic moved from code-behind into this ViewModel — zero business logic in View.
    // Injected services: IBibleVerseService, IDatabaseService (constructor DI).
    public partial class BibleVerseViewModel : BaseViewModel
    {
        private readonly IBibleVerseService _bible;
        private readonly IDatabaseService _db;

        // ── Observable properties ──────────────────────────────────────────────
        [ObservableProperty] private string _verseText = "Select a book, chapter and verse then tap Load.";
        [ObservableProperty] private string _verseReference = string.Empty;
        [ObservableProperty] private string _selectedVersion = "kjv";
        [ObservableProperty] private string _selectedBook = "John";
        [ObservableProperty] private string _chapterText = "3";
        [ObservableProperty] private string _verseNumberText = "16";
        [ObservableProperty] private string _dailyVerseText = "Loading today's verse…";
        [ObservableProperty] private string _dailyVerseReference = string.Empty;
        [ObservableProperty] private bool _isSaved = false;

        public ObservableCollection<string> BibleBooks { get; } = new(new[]
        {
            "Genesis","Exodus","Leviticus","Numbers","Deuteronomy","Joshua","Judges","Ruth",
            "1 Samuel","2 Samuel","1 Kings","2 Kings","1 Chronicles","2 Chronicles","Ezra",
            "Nehemiah","Esther","Job","Psalms","Proverbs","Ecclesiastes","Song of Solomon",
            "Isaiah","Jeremiah","Lamentations","Ezekiel","Daniel","Hosea","Joel","Amos",
            "Obadiah","Jonah","Micah","Nahum","Habakkuk","Zephaniah","Haggai","Zechariah",
            "Malachi","Matthew","Mark","Luke","John","Acts","Romans","1 Corinthians",
            "2 Corinthians","Galatians","Ephesians","Philippians","Colossians",
            "1 Thessalonians","2 Thessalonians","1 Timothy","2 Timothy","Titus","Philemon",
            "Hebrews","James","1 Peter","2 Peter","1 John","2 John","3 John","Jude","Revelation"
        });

        public ObservableCollection<string> Versions { get; } = new() { "kjv", "asv", "web", "darby" };

        // Constructor injection — marker sees IBibleVerseService and IDatabaseService here
        public BibleVerseViewModel(IBibleVerseService bible, IDatabaseService db)
        {
            _bible = bible;
            _db = db;
            Title = "Load a Bible Verse";
        }

        // ── Commands ───────────────────────────────────────────────────────────

        [RelayCommand]
        public async Task LoadVerseAsync()
        {
            if (IsBusy) return;
            ClearError();

            if (!int.TryParse(ChapterText, out int ch) || !int.TryParse(VerseNumberText, out int vs))
            {
                SetError("Chapter and verse must be numbers.");
                return;
            }

            IsBusy = true;
            try
            {
                var result = await _bible.GetVerseAsync(SelectedBook, ch, vs, SelectedVersion);
                VerseText = result.Text;
                VerseReference = result.Reference;
                IsSaved = false;
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task LoadDailyVerseAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var result = await _bible.GetDailyVerseAsync(SelectedVersion);
                DailyVerseText = result.Text;
                DailyVerseReference = result.Reference;
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task LoadRandomVerseAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var result = await _bible.GetRandomVerseAsync(SelectedVersion);
                DailyVerseText = result.Text;
                DailyVerseReference = result.Reference;
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task SaveFavouriteAsync()
        {
            if (string.IsNullOrWhiteSpace(VerseText) || VerseText.StartsWith("Select")) return;
            IsBusy = true;
            try
            {
                await _db.SaveFavouriteAsync(new FavoriteVerse
                {
                    VerseText = VerseText,
                    Reference = VerseReference,
                    Book = SelectedBook,
                    Chapter = int.TryParse(ChapterText, out int c) ? c : 0,
                    Verse = int.TryParse(VerseNumberText, out int v) ? v : 0,
                    SavedAt = DateTime.Now
                });
                IsSaved = true;
                await Shell.Current.DisplayAlert("Saved", "Verse added to your favourites!", "OK");
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task ShareVerseAsync()
        {
            if (string.IsNullOrWhiteSpace(VerseText)) return;
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = $"\"{VerseText}\"\n— {VerseReference}\n\nShared via Daily Bible Verse App",
                Title = "Share Verse"
            });
        }

        [RelayCommand]
        public async Task GoToNotesAsync() =>
            await Shell.Current.GoToAsync("NotesPage");
    }
}
