using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Clean.Application.Abstractions;
using Clean.Application.Dtos;
using Clean.Application.Responses;
using Clean.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace Clean.Application.Services;

public class AuthenticationService(IAuthenticationRepository repository,IConfiguration configuration):IAuthenticationService
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
        var smtpHost = Environment.GetEnvironmentVariable("MAILTRAP_HOST") ?? "sandbox.smtp.mailtrap.io";
        var smtpPort = int.Parse(Environment.GetEnvironmentVariable("MAILTRAP_PORT") ?? "587");
        var smtpUser = Environment.GetEnvironmentVariable("MAILTRAP_USER") ?? "7cca393f713483";
        var smtpPass = Environment.GetEnvironmentVariable("MAILTRAP_PASS") ?? "3354d871180949";

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var verificationLink = $"https://believable-wisdom-production.up.railway.app/api/VerifyEmail?token={token.Id}";

        var mailMessage = new MailMessage
        {
            From = new MailAddress("no-reply@yourapp.com", "TheApp"), 
            Subject = "Confirm your email",
            Body = $"<p>Hello {model.FullName},</p><p>Click the link below to verify your email:</p><a href='{verificationLink}'>Verify Email</a>",
            IsBodyHtml = true
        };
        
        mailMessage.To.Add(model.Email);

        await client.SendMailAsync(mailMessage);
        Console.WriteLine("✅ Test email sent successfully to Mailtrap!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Failed to send email: {ex.Message}");
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