
using Clean.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Clean.Application.Services;

public static class UserEndpoints
{
    private const string Tag = "Users";
    public const string VerifyEmail = "Verify";
    
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("users/verify-email", async (Guid token, VerifyEmail useCase) =>
            {
                var success = await useCase.Handle(token);
                return success == Status.Active 
                    ? (IResult)Results.Ok("Verification successful!") 
                    : (IResult)Results.BadRequest("Verification token expired or invalid!");
            })
            .WithTags(Tag)
            .WithName(VerifyEmail);

        return builder;
    }
}