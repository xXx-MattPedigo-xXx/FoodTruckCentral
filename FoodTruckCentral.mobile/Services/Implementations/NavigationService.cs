using FoodTruckCentral.Mobile.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTruckCentral.Mobile.Services.Implementations
{
    public class NavigationService : INavigationService
    {
        protected readonly IAuthService _authService;

        public Shell CurrentShell => Shell.Current;

        public NavigationService(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task InitializeAsync()
        {
            // Check if user is authenticated and navigate accordingly
            if (await _authService.IsAuthenticatedAsync())
            {
                await NavigateToMainAsync();
            }
            else
            {
                await NavigateToLoginAsync();
            }
        }

        public Task NavigateToAsync(string route)
        {
            if (string.IsNullOrEmpty(route))
                return Task.CompletedTask;

            return CurrentShell.GoToAsync(route);
        }

        public Task NavigateToAsync(string route, IDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(route))
                return Task.CompletedTask;

            return CurrentShell.GoToAsync(route, parameters);
        }

        public Task GoBackAsync()
        {
            if (CurrentShell.Navigation.NavigationStack.Count > 1)
            {
                return CurrentShell.GoToAsync("..");
            }

            return Task.CompletedTask;
        }

        public Task NavigateToMainAsync()
        {
            return NavigateToAsync($"///{nameof(MainPage)}");
        }

        public Task NavigateToLoginAsync()
        {
            return NavigateToAsync($"///{nameof(LoginPage)}");
        }
    }
}
