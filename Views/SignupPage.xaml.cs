using DailyBibleVerseApp.ViewModels;

namespace DailyBibleVerseApp.Views;

// SignupPage is now merged into LoginPage via toggle.
// This class remains for routing compatibility only.
public partial class SignupPage : ContentPage
{
    public SignupPage()
    {
        // Redirect to LoginPage with register mode
        _ = Shell.Current.GoToAsync("//LoginPage");
    }
}
