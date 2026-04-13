using DailyBibleVerseApp.Interfaces;
using DailyBibleVerseApp.Views;

namespace DailyBibleVerseApp;

// App: application entry point.
// CS0618 fix: Application.MainPage is obsolete in .NET MAUI 9.
// Use CreateWindow() override instead.
public partial class App : Application
{
    private readonly IAuthService _auth;
    private readonly IDatabaseService _db;

    public App(IAuthService auth, IDatabaseService db)
    {
        InitializeComponent();
        _auth = auth;
        _db = db;
    }

    // Replaces obsolete MainPage = new AppShell()
    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(new AppShell());

    protected override async void OnStart()
    {
        base.OnStart();

        // Initialise SQLite tables on first run
        await _db.InitialiseAsync();

        // Register shell routes for modal navigation
        Routing.RegisterRoute(nameof(NotesPage), typeof(NotesPage));

        // Restore previous session or go to login
        bool resumed = await _auth.TryAutoLoginAsync();
        if (resumed)
            await Shell.Current.GoToAsync("//MainPage");
        else
            await Shell.Current.GoToAsync("//LoginPage");
    }
}
