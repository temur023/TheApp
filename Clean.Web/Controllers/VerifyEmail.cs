

using Clean.Application.Abstractions;
using Clean.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

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

            return status switch
            {
                Status.Active => Ok("Email verified successfully!"),
                Status.Blocked => BadRequest("User is blocked."),
                Status.Unverified => BadRequest("Invalid or expired token."),
                _ => BadRequest("Unknown error")
            };
        }
    }
