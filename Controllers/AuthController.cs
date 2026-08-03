using Microsoft.AspNetCore.Mvc;
using ToDoApp.Api.DTOs.Auth;
using ToDoApp.Api.Services.Interfaces;

namespace ToDoApp.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var result = await authService.RegisterAsync(dto);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        return Ok(result);
    }
}
