using CommunityToolkit.Mvvm.ComponentModel;

namespace DailyBibleVerseApp.ViewModels
{
    /// <summary>
    /// Base ViewModel inherited by all ViewModels.
    /// Provides IsBusy, Title, ErrorMessage, HasError shared state.
    /// DRY principle: written once, used by all ViewModels.
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        public bool IsNotBusy => !IsBusy;

        protected void SetError(string msg)
        {
            ErrorMessage = msg;
            HasError = true;
        }

        protected void ClearError()
        {
            ErrorMessage = string.Empty;
            HasError = false;
        }
    }
}
