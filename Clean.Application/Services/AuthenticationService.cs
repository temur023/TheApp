using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clean.Application.Abstractions;
using Clean.Application.Dtos;
using Clean.Application.Responses;
using Clean.Domain.Entities;
using FluentEmail.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Clean.Application.Services;

public class AuthenticationService(IAuthenticationRepository repository,IConfiguration configuration,
    IFluentEmail fluentEmail, EmailVerificationLinkFactory linkFactory):IAuthenticationService
{

    public async Task<Response<string>> Register(UserCreateDto user)
    {
        try
        {
            var model = new User
            {
                FullName = user.FullName,
                Email = user.Email,
                Password = user.Password,
                DateOfRegistration = DateTimeOffset.UtcNow,
                LastSeen = DateTimeOffset.UtcNow,
                Status = Status.Unverified
            };
            if (model.Password.Length < 1)
                return new Response<string>(400, "Password should contain al least a charecter");
            if (model.Password != user.CheckPassword)
                return new Response<string>(400, "Passwords do not match!");
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            var verificationToken = new EmailVerificationToken()
            {
                Id = Guid.NewGuid(),
                UserId = model.Id,
                Created = utcNow,
                Expires = utcNow.AddDays(1)
            };
            await repository.Register(model,verificationToken);
            string verificationLink = linkFactory.Create(verificationToken);

            try 
            {
                await fluentEmail
                    .To(user.Email)
                    .Subject("Confirmation for login!")
                    .Body($"<a href='{verificationLink}'>Click here</a> to confirm!", true)
                    .SendAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }

            return new Response<string>(200, "The user has been registered!");
        }catch(Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return new Response<string>(400, "User with this email already exists!");
        }
       
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