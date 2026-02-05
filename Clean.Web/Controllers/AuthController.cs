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
        var response = await service.Register(user);
        if (response.StatusCode != 200)
        {
            return StatusCode(response.StatusCode, response);
        }
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto user)
    {
        var response = await service.Login(user);
        if (response.StatusCode != 200)
        {
            return StatusCode(response.StatusCode, response);
        }
        return Ok(response);
    }
}