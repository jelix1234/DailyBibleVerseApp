using DailyBibleVerseApp.ViewModels;

namespace DailyBibleVerseApp.Views;

public partial class DailyVersePage : ContentPage
{
    private readonly BibleVerseViewModel _vm;

    public DailyVersePage(BibleVerseViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Set date label (avoids sys:DateTime XAML issue)
        if (FindByName("DateLabel") is Label lbl)
            lbl.Text = DateTime.Now.ToString("dddd, MMMM d");
        await _vm.LoadDailyVerseCommand.ExecuteAsync(null);
    }
}
