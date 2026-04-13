using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyBibleVerseApp.Interfaces;
using DailyBibleVerseApp.Models;
using System.Collections.ObjectModel;

namespace DailyBibleVerseApp.ViewModels
{
    // FavouritesViewModel: loads, shares, and deletes saved verses.
    public partial class FavouritesViewModel : BaseViewModel
    {
        private readonly IDatabaseService _db;

        [ObservableProperty] private ObservableCollection<FavoriteVerse> _favourites = new();
        [ObservableProperty] private bool _isEmpty;

        public FavouritesViewModel(IDatabaseService db)
        {
            _db = db;
            Title = "Favourites";
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var list = await _db.GetFavouritesAsync();
                Favourites = new ObservableCollection<FavoriteVerse>(list);
                IsEmpty = Favourites.Count == 0;
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task DeleteAsync(FavoriteVerse verse)
        {
            bool ok = await Shell.Current.DisplayAlert("Remove", "Remove from favourites?", "Remove", "Cancel");
            if (!ok) return;
            await _db.DeleteFavouriteAsync(verse.Id);
            Favourites.Remove(verse);
            IsEmpty = Favourites.Count == 0;
        }

        [RelayCommand]
        private async Task ClearAllAsync()
        {
            bool ok = await Shell.Current.DisplayAlert("Clear", "Remove all favourites?", "Clear", "Cancel");
            if (!ok) return;
            await _db.ClearFavouritesAsync();
            Favourites.Clear();
            IsEmpty = true;
        }

        [RelayCommand]
        private async Task ShareAsync(FavoriteVerse verse)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = $"\"{verse.VerseText}\"\n— {verse.Reference}\n\nShared via Daily Bible Verse App",
                Title = "Share Verse"
            });
        }
    }
}
