using DailyBibleVerseApp.ViewModels;

namespace DailyBibleVerseApp.Views;

public partial class NotesPage : ContentPage
{
    private readonly NotesViewModel _vm;

    public NotesPage(NotesViewModel vm)
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
