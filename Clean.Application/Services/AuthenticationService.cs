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
    // 1. Validation
    if (string.IsNullOrWhiteSpace(user.Password))
        return new Response<string>(400, "Password is required.");

    if (user.Password != user.CheckPassword)
        return new Response<string>(400, "Passwords do not match.");

    // 2. Create User Model
    var model = new User
    {
        FullName = user.FullName,
        Email = user.Email,
        Password = user.Password, // Note: In production, ALWAYS hash this password!
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

    // 3. Save to Database
    try
    {
        await repository.Register(model, token);
    }
    catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
    {
        return new Response<string>(400, "Email already registered");
    }

    // 4. Send Email via Gmail
    try
    {
        // Get vars from Environment (populated from Railway)
        var smtpHost = Environment.GetEnvironmentVariable("MAILTRAP_HOST");
        var smtpPort = int.Parse(Environment.GetEnvironmentVariable("MAILTRAP_PORT") ?? "587");
        var smtpUser = Environment.GetEnvironmentVariable("MAILTRAP_USER");
        var smtpPass = Environment.GetEnvironmentVariable("MAILTRAP_PASS");
        var baseUrl  = Environment.GetEnvironmentVariable("AppBaseUrl"); // Make sure this is set in Railway!

        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPass))
        {
            Console.WriteLine("❌ SMTP Configuration missing.");
            return new Response<string>(500, "Server email configuration error.");
        }

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true, // REQUIRED for Gmail
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        // Construct the correct verification link
        // Adjust the path "/api/Auth/verify..." based on your actual Controller route
        var verificationLink = $"{baseUrl.TrimEnd('/')}/api/VerifyEmail?token={token.Id}";

        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpUser, "TheApp Support"), // Gmail requires 'From' to match the authenticated user
            Subject = "Confirm your email",
            Body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2>Hello {model.FullName},</h2>
                    <p>Thank you for registering. Please click the button below to verify your email address:</p>
                    <a href='{verificationLink}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Verify Email</a>
                    <p style='margin-top:20px; color: #888;'>If the button doesn't work, copy this link: <br>{verificationLink}</p>
                </div>",
            IsBodyHtml = true
        };
        
        mailMessage.To.Add(model.Email);

        await client.SendMailAsync(mailMessage);
        Console.WriteLine($"✅ Email sent to {model.Email} via Gmail!");
    }
    catch (Exception ex)
    {
        // Log the full error to Railway logs so you can debug
        Console.WriteLine($"❌ Failed to send email: {ex.Message} | {ex.InnerException?.Message}");
        // Optional: Don't fail the request if email fails, or return 500 depending on requirements
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