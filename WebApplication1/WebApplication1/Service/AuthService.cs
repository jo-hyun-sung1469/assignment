using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Model;
using WebApplication1.Repository;
using LoginRequest = Microsoft.AspNetCore.Identity.Data.LoginRequest;

namespace WebApplication1.Service;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }
    
    public async Task<LoginResult> LoginAsync(LoginRequest loginRequest)
    {
        try
        {
            var user = await _userRepository.FindByUsernameAsync(loginRequest.Username);
            if (user is null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
            {
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "아이디 또는 비밀번호가 올바르지 않습니다."
                };
            }

            var token = GenerateJwtToken(user);
            return new LoginResult
            {
                Success = true,
                Token = token
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "로그인 처리 중 오류 발생");
            return new LoginResult { Success = false,  ErrorMessage = "로그인 처리 중 오류가 발생했습니다." };
        }
    }

    public Task LogoutAsync(string userId)
    {
        _logger.LogInformation("로그아웃 완료");
        return Task.CompletedTask;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtConfig = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name)
        };
        var token = new JwtSecurityToken(
            issuer: jwtConfig["Issuer"],
            audience: jwtConfig["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtConfig["ExpiresInMinutes"])),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}