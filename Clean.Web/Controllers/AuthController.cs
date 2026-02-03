using Clean.Application.Abstractions;
using Clean.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace loginForm.Controllers;

[Route("api/[controller]")]
[ApiController]

public class AuthController(IAuthenticationService service):ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Register(UserCreateDto user)
    {
        return Ok(await service.Register(user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto user)
    {
        return Ok(await service.Login(user));
    }
}