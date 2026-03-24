using WebApplication1.Model;

namespace WebApplication1.Service;

public interface IAuthService
{
    Task<LoginResult>  LoginAsync(LoginRequest loginRequest);
    Task LogoutAsync(string token);
    Task<bool> DeleteAccountAsync(long userId);
    Task<UserResult?> GetUserAsync(long userId);
    Task<bool> RegisterAsync(string username, string password);
}