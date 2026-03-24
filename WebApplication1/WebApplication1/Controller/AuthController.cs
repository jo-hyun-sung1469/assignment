using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Service;

namespace WebApplication1.Controller;
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Regiser([FromBody] LoginRequest request)
    {
        var result = await _authService.RegisterAsync(request.Username, request.Password);
        if (!result)
            return Conflict("이미 존재하는 사용자입니다");
        return Ok(result);
    }
    

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.Success)
            return Unauthorized(result.ErrorMessage);

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromQuery] string userId)
    {
        await _authService.LogoutAsync(userId);
        return Ok("로그아웃 완료");
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteAccount([FromRoute]long userId)
    {
        var result = await _authService.DeleteAccountAsync(userId);
        if (!result)
        {
            return NotFound("사용자를 찾을수 없습니다");
        }

        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser([FromRoute]long userId)
    {
        var result = await _authService.GetUserAsync(userId);
        if (result is null)
        {
            return NotFound("사용자를 찾을수 없습니다");
        }
        return Ok(result);
    }
}