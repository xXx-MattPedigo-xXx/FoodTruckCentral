using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodTruckCentral.Mobile.Services.Interfaces;
using FoodTruckCentral.Mobile.ViewModels;
using FoodTruckCentral.Mobile.Services.Interfaces;

namespace FoodTruckCentral.Mobile.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isError;

        [ObservableProperty]
        private string errorMessage;

        public LoginViewModel(
            IAuthService authService,
            INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            Title = "Login";
        }

        [RelayCommand]
        async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                IsError = false;
                ErrorMessage = string.Empty;

                var result = await _authService.LoginAsync(Email, Password);
                if (result.Succeeded)
                {
                    await _navigationService.NavigateToMainAsync();
                }
                else
                {
                    IsError = true;
                    ErrorMessage = result.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                IsError = true;
                ErrorMessage = "An error occurred during login.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task LoginWithGoogleAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                IsError = false;
                ErrorMessage = string.Empty;

                var result = await _authService.LoginWithGoogleAsync();
                if (result.Succeeded)
                {
                    await _navigationService.NavigateToMainAsync();
                }
                else
                {
                    IsError = true;
                    ErrorMessage = result.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                IsError = true;
                ErrorMessage = "An error occurred during Google login.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task NavigateToRegisterAsync()
        {
            await _navigationService.NavigateToAsync("RegisterPage");
        }
    }
}