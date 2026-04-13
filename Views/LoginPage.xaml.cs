using DailyBibleVerseApp.ViewModels;

namespace DailyBibleVerseApp.Views;

// Zero business logic — ViewModel injected via DI, bound in constructor.
public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
