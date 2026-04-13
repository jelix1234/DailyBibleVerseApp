using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyBibleVerseApp.Interfaces;

namespace DailyBibleVerseApp.ViewModels
{
    // LoginViewModel: handles login and register from a single page (toggle mode).
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _auth;

        [ObservableProperty] private string _username = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private bool _isRegistering = false;

        public LoginViewModel(IAuthService auth)
        {
            _auth = auth;
            Title = "Welcome";
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;
            ClearError();
            IsBusy = true;
            try
            {
                var (ok, msg) = await _auth.LoginAsync(Username, Password);
                if (ok)
                    await Shell.Current.GoToAsync("//MainPage");
                else
                    SetError(msg);
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy) return;
            ClearError();
            IsBusy = true;
            try
            {
                var (ok, msg) = await _auth.RegisterAsync(Username, Password);
                if (ok)
                    await Shell.Current.GoToAsync("//MainPage");
                else
                    SetError(msg);
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private void ToggleMode()
        {
            IsRegistering = !IsRegistering;
            Title = IsRegistering ? "Create Account" : "Welcome";
            ClearError();
        }
    }
}
