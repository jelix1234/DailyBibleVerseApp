using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyBibleVerseApp.Interfaces;
using DailyBibleVerseApp.Models;
using System.Collections.ObjectModel;

namespace DailyBibleVerseApp.ViewModels
{
    // NotesViewModel: full CRUD for Notes, search, linked verse.
    // Replaces the old Preferences-only version with real SQLite backing.
    public partial class NotesViewModel : BaseViewModel
    {
        private readonly IDatabaseService _db;
        private List<Note> _allNotes = new();

        [ObservableProperty] private ObservableCollection<Note> _notes = new();
        [ObservableProperty] private bool _isEmpty;
        [ObservableProperty] private string _searchText = string.Empty;

        // For the add/edit form
        [ObservableProperty] private string _noteTitle = string.Empty;
        [ObservableProperty] private string _noteContent = string.Empty;
        [ObservableProperty] private string _linkedVerse = string.Empty;
        [ObservableProperty] private bool _isEditing;
        [ObservableProperty] private int _editingNoteId;

        // Constructor injection
        public NotesViewModel(IDatabaseService db)
        {
            _db = db;
            Title = "My Notes";
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                _allNotes = await _db.GetNotesAsync();
                Notes = new ObservableCollection<Note>(_allNotes);
                IsEmpty = Notes.Count == 0;
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private void Search()
        {
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allNotes
                : _allNotes.Where(n =>
                    n.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    n.Text.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            Notes = new ObservableCollection<Note>(filtered);
            IsEmpty = Notes.Count == 0;
        }

        [RelayCommand]
        private void PrepareNewNote()
        {
            IsEditing = false;
            EditingNoteId = 0;
            NoteTitle = string.Empty;
            NoteContent = string.Empty;
            LinkedVerse = string.Empty;
            ClearError();
        }

        [RelayCommand]
        private void PrepareEditNote(Note note)
        {
            IsEditing = true;
            EditingNoteId = note.Id;
            NoteTitle = note.Title;
            NoteContent = note.Text;
            LinkedVerse = note.LinkedVerse ?? string.Empty;
            ClearError();
        }

        [RelayCommand]
        private async Task SaveNoteAsync()
        {
            if (string.IsNullOrWhiteSpace(NoteTitle))
            {
                SetError("Please enter a title.");
                return;
            }
            IsBusy = true;
            try
            {
                Note note;
                if (IsEditing && EditingNoteId > 0)
                {
                    note = await _db.GetNoteByIdAsync(EditingNoteId) ?? new Note();
                }
                else
                {
                    note = new Note { CreatedAt = DateTime.Now };
                }
                note.Title = NoteTitle.Trim();
                note.Text = NoteContent.Trim();
                note.LinkedVerse = string.IsNullOrWhiteSpace(LinkedVerse) ? null : LinkedVerse.Trim();
                await _db.SaveNoteAsync(note);
                PrepareNewNote();
                await LoadAsync();
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task DeleteNoteAsync(Note note)
        {
            bool ok = await Shell.Current.DisplayAlert("Delete", $"Delete \"{note.Title}\"?", "Delete", "Cancel");
            if (!ok) return;
            await _db.DeleteNoteAsync(note.Id);
            _allNotes.Remove(note);
            Notes.Remove(note);
            IsEmpty = Notes.Count == 0;
        }
    }
}
