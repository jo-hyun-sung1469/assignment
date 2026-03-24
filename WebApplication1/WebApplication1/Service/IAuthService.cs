using WebApplication1.Model;
using LoginRequest = Microsoft.AspNetCore.Identity.Data.LoginRequest;

namespace WebApplication1.Service;

public interface IAuthService
{
    Task<LoginResult>  LoginAsync(LoginRequest loginRequest);
    Task LogoutAsync(string token);
}