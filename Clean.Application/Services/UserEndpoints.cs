namespace Clean.Application.Services;
using Clean.Application.Abstractions;
using Clean.Application.Dtos;
using Clean.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class UserEndpoints
{
    private const string Tag = "Auth";

    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/Auth/create", async (UserCreateDto user, IAuthenticationService service) =>
        {
            var result = await service.Register(user);
            return Results.Json(result, statusCode: result.StatusCode);
        }).WithTags("Auth");

        builder.MapPost("api/Auth/login", async (UserLoginDto user, IAuthenticationService service) =>
        {
            var result = await service.Login(user);
            return Results.Json(result, statusCode: result.StatusCode);
        }).WithTags("Auth");

        return builder;
    }
}