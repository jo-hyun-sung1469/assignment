using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Model;
using WebApplication1.Repository;

namespace WebApplication1.Service;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthRepository authRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _authRepository = authRepository;
        _configuration = configuration;
        _logger = logger;
    }
    
    public async Task<LoginResult> LoginAsync(LoginRequest loginRequest)
    {
        try
        {
            var user = await _authRepository.FindByUsernameAsync(loginRequest.Username);
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
    

    public Task LogoutAsync(string token)
    {
        _logger.LogInformation("로그아웃 완료");
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAccountAsync(long userId)
    {
        try
        {
            var user = await _authRepository.FindByIdAsync(userId);
            if (user is null)
                return false;

            await _authRepository.DeleteAsync(user);
            _logger.LogInformation("계정 삭제 완료 - UserId: {UserId}", userId);
            return true;
        }
        catch(Exception e)
        {
            _logger.LogError(e, "계정 삭제 중 오류 발생 - UserId: {UserId}", userId);
            return false;
        }
    }

    public async Task<UserResult?> GetUserAsync(long userId)
    {
        try
        {
            var user = await _authRepository.FindByIdAsync(userId);
            if(user is null)
                return null;

            return new UserResult
            {
                Id = user.Id,
                Name = user.Name
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "계정 조회 중 오류 발생 - UserId: {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        try
        {
            var existing = await _authRepository.FindByUsernameAsync(username);
            if (existing is not null)
                return false;
            await _authRepository.AddAsync(new User{Id = new Random().NextInt64(),Name = username, PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)});
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "계정 생성 중 오류 발생");
            return false;
        }
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