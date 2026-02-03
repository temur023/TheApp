using Clean.Domain.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Clean.Application.Services;

public class EmailVerificationLinkFactory(IHttpContextAccessor httpContextAccessor,
    LinkGenerator linkGenerator)
{
    public string Create(EmailVerificationToken verificationToken)
    {
        string? verifactionLink = linkGenerator.GetUriByName(
            httpContextAccessor.HttpContext!,
            UserEndpoints.VerifyEmail,
            new { token = verificationToken.Id });
        return verifactionLink ?? "Couldn't Create Verfication Link!";
    }
}