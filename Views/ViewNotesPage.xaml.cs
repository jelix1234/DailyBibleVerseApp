namespace DailyBibleVerseApp.Views;

// ViewNotesPage merged into NotesPage. Kept as stub for compile compatibility.
public partial class ViewNotesPage : ContentPage
{
    public ViewNotesPage()
    {
        _ = Shell.Current.GoToAsync("//NotesPage");
    }
}
