using Clean.Application.Abstractions;
using Clean.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace loginForm.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VerifyEmailController : ControllerBase
{
    private readonly IVerifyEmailService _verifyEmailService;

    public VerifyEmailController(IVerifyEmailService verifyEmailService)
    {
        _verifyEmailService = verifyEmailService;
    }

    [HttpGet]
    public async Task<IActionResult> Verify([FromQuery] Guid token)
    {
        var status = await _verifyEmailService.Handle(token);
            
        string frontendUrl = "https://theapp-production-3330.up.railway.app/login";

        return status switch
        {
            Status.Active => Redirect($"{frontendUrl}?status=success"),
            Status.Blocked => BadRequest("Your account is blocked."),
            Status.Unverified => BadRequest("The link is invalid or has expired."),
            _ => BadRequest("An unexpected error occurred.")
        };
    }
}