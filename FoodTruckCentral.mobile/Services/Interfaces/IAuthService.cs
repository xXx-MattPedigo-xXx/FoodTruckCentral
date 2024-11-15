using System.Threading.Tasks;

namespace FoodTruckCentral.Mobile.Services.Interfaces
{
    public interface IAuthService
    {
        bool IsLoggedIn { get; }
        Task<AuthResult> LoginAsync(string email, string password);
        Task<AuthResult> LoginWithGoogleAsync();
        Task<AuthResult> RegisterAsync(string email, string password, string firstName, string lastName);
        Task LogoutAsync();
        Task<string> GetCurrentUserTokenAsync();
        Task<UserInfo> GetCurrentUserAsync();
        Task<bool> IsAuthenticatedAsync();
    }

    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
        public required string ErrorMessage { get; set; }
        public required UserInfo User { get; set; }
    }

    public class UserInfo
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? PictureUrl { get; set; }
    }
}