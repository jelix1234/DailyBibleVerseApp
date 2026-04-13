using DailyBibleVerseApp.ViewModels;

namespace DailyBibleVerseApp.Views;

public partial class FavoritesPage : ContentPage
{
    private readonly FavouritesViewModel _vm;

    public FavoritesPage(FavouritesViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
