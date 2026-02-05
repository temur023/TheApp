using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clean.Application.Abstractions;
using Clean.Application.Dtos;
using Clean.Application.Responses;
using Clean.Domain.Entities;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Resend;

namespace Clean.Application.Services;

public class AuthenticationService(IAuthenticationRepository repository,IConfiguration configuration,
    IFluentEmail fluentEmail, EmailVerificationLinkFactory linkFactory):IAuthenticationService
{

public async Task<Response<string>> Register(UserCreateDto user)
{
    if (string.IsNullOrWhiteSpace(user.Password))
        return new Response<string>(400, "Password is required.");

    if (user.Password != user.CheckPassword)
        return new Response<string>(400, "Passwords do not match.");

    var model = new User
    {
        FullName = user.FullName,
        Email = user.Email,
        Password = user.Password,
        DateOfRegistration = DateTimeOffset.UtcNow,
        LastSeen = DateTimeOffset.UtcNow,
        Status = Status.Unverified
    };

    var token = new EmailVerificationToken
    {
        Id = Guid.NewGuid(),
        UserId = model.Id,
        Created = DateTimeOffset.UtcNow,
        Expires = DateTimeOffset.UtcNow.AddDays(1)
    };

    try
    {
        await repository.Register(model, token);
    }
    catch (DbUpdateException ex) when (
        ex.InnerException is PostgresException pg &&
        pg.SqlState == "23505")
    {
        return new Response<string>(400, "Email already registered");
    }
    
    try
    {
        var apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY")!;
        var resend = ResendClient.Create(apiKey);

        var verificationLink = $"https//:believable-wisdom-production.up.railway.app/api/VerifyEmail?token={token.Id}";

        var response = await resend.EmailSendAsync(new EmailMessage
        {
            From = "temurmalikirgashev@gmail.com",
            To = model.Email,
            Subject = "Confirm your email",
            HtmlBody = $"<p>Hello {model.FullName},</p><p>Click the link below to verify your email:</p><a href='{verificationLink}'>Verify Email</a>"
        });

        Console.WriteLine($"Email sent via Resend, ID: {response}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to send email via Resend: {ex.Message}");
    }

    return new Response<string>(200, "Registration complete. Verification email sent!");
}

    public async Task<Response<string>> Login(UserLoginDto user)
    {
        var usr = await repository.Login(user.Email);
        if (usr == null) 
            return new Response<string>(404, "User not found!");
        if (usr.Password != user.Password)
            return new Response<string>(400, "The password is incorrect!");
        if (usr.Status == Status.Blocked)
            return new Response<string>(400, "You cannot login because you are blocked!");
        var token = CreateToken(usr);
        usr.LastSeen = DateTimeOffset.UtcNow;
        await repository.Update();
        return new Response<string>(200, token);
    }
    
    
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor); 
    }
}