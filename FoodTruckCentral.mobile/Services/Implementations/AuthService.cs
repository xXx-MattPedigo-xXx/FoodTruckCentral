using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FoodTruckCentral.Mobile.Services.Interfaces;

namespace FoodTruckCentral.Mobile.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ISecureStorage _secureStorage;
        private readonly IPreferences _preferences;
        private const string TokenKey = "auth_token";
        private const string RefreshTokenKey = "refresh_token";
        private const string UserKey = "user_info";

        public AuthService(
            HttpClient httpClient,
            ISecureStorage secureStorage,
            IPreferences preferences)
        {
            _httpClient = httpClient;
            _secureStorage = secureStorage;
            _preferences = preferences;
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(
            _secureStorage.GetAsync(TokenKey).Result);

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new
                {
                    email,
                    password
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResult>();
                    await StoreAuthDataAsync(result);
                    return result;
                }

                var error = await response.Content.ReadAsStringAsync();
                return new AuthResult
                {
                    Succeeded = false,
                    Token = string.Empty,
                    RefreshToken = string.Empty,
                    ErrorMessage = error,
                    User = null
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Token = string.Empty,
                    RefreshToken = string.Empty,
                    ErrorMessage = "An error occurred during login.",
                    User = null
                };
            }
        }

        public async Task<AuthResult> LoginWithGoogleAsync()
        {
            try
            {
                var webAuthenticator = CrossWebAuthenticator.Current;
                var authUrl = new Uri("your-google-auth-url");
                var callbackUrl = new Uri("your-callback-url");

                var result = await webAuthenticator.AuthenticateAsync(
                    authUrl,
                    callbackUrl);

                if (result?.AccessToken != null)
                {
                    var response = await _httpClient.PostAsJsonAsync(
                        "/api/auth/google",
                        new { token = result.AccessToken });

                    if (response.IsSuccessStatusCode)
                    {
                        var authResult = await response.Content
                            .ReadFromJsonAsync<AuthResult>();
                        await StoreAuthDataAsync(authResult);
                        return authResult;
                    }
                }

                return new AuthResult
                {
                    Succeeded = false,
                    Token = string.Empty,
                    RefreshToken = string.Empty,
                    ErrorMessage = "Google authentication failed.",
                    User = null
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Token = string.Empty,
                    RefreshToken = string.Empty,
                    ErrorMessage = "An error occurred during Google login.",
                    User = null
                };
            }
        }

        public async Task<AuthResult> RegisterAsync(
            string email,
            string password,
            string firstName,
            string lastName)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/register", new
                {
                    email,
                    password,
                    firstName,
                    lastName
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content
                        .ReadFromJsonAsync<AuthResult>();
                    await StoreAuthDataAsync(result);
                    return result;
                }

                var error = await response.Content.ReadAsStringAsync();
                return new AuthResult
                {
                    Succeeded = false,
                    Token = string.Empty,
                    RefreshToken = string.Empty,
                    ErrorMessage = error,
                    User = null
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Token = string.Empty,
                    RefreshToken = string.Empty,
                    ErrorMessage = "An error occurred during registration.",
                    User = null
                };
            }
        }

        public async Task LogoutAsync()
        {
            await _secureStorage.Remove(TokenKey);
            await _secureStorage.Remove(RefreshTokenKey);
            _preferences.Remove(UserKey);
        }

        public async Task<string> GetCurrentUserTokenAsync()
        {
            return await _secureStorage.GetAsync(TokenKey);
        }

        public async Task<UserInfo> GetCurrentUserAsync()
        {
            var userJson = _preferences.Get(UserKey, string.Empty);
            if (string.IsNullOrEmpty(userJson))
                return null;

            return System.Text.Json.JsonSerializer
                .Deserialize<UserInfo>(userJson);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetCurrentUserTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            // Check if token is expired
            try
            {
                var response = await _httpClient.GetAsync("/api/auth/validate");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task StoreAuthDataAsync(AuthResult result)
        {
            if (result?.Succeeded == true)
            {
                await _secureStorage.SetAsync(TokenKey, result.Token);

                if (!string.IsNullOrEmpty(result.RefreshToken))
                    await _secureStorage.SetAsync(RefreshTokenKey, result.RefreshToken);

                if (result.User != null)
                {
                    var userJson = System.Text.Json.JsonSerializer
                        .Serialize(result.User);
                    _preferences.Set(UserKey, userJson);
                }
            }
        }
    }
}