using CommunityToolkit.Maui;
//using FoodTruckCentral.mobile.Services.Implementations;
using FoodTruckCentral.Mobile.Services.Interfaces;
using FoodTruckCentral.Mobile.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Maui.Toolkit.Hosting;

namespace FoodTruckCentral.Mobile
{
    namespace FoodTruckCentral.Mobile
    {
public static class MauiProgram
        {
            public static MauiApp CreateMauiApp()
            {
                var builder = MauiApp.CreateBuilder();
                builder
                    .UseMauiApp<App>()
                    .UseMauiCommunityToolkit() // Added this line to chain UseMauiCommunityToolkit
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    });

                // Register HttpClient
                builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
                {
                    client.BaseAddress = new Uri("https://your-api-url.com");
                });

                // Register services
                builder.Services.AddSingleton<ISecureStorage>(SecureStorage.Default);
                builder.Services.AddSingleton<IPreferences>(Preferences.Default);
                builder.Services.AddSingleton<IAuthService, AuthService>();

                // Register pages and viewmodels
                builder.Services.AddTransient<LoginPage>();
                builder.Services.AddTransient<LoginViewModel>();
                builder.Services.AddTransient<RegisterPage>();
                builder.Services.AddTransient<RegisterViewModel>();

                return builder.Build();
            }
        }
    }
