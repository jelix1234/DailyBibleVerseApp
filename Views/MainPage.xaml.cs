using DailyBibleVerseApp.ViewModels;

namespace DailyBibleVerseApp.Views;

// Zero business logic. ViewModel injected via DI.
public partial class MainPage : ContentPage
{
    private readonly BibleVerseViewModel _vm;

    public MainPage(BibleVerseViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }
}
