using Clean.Application.Abstractions;
using Clean.Application.Services;
using Clean.Infrastructure.Data;
using Clean.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clean.Infrastructure;

public static class RegisterDependencies
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        services.AddDbContext<DataContext>(options=>
            options.UseNpgsql(connectionString));
        services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVerifyEmailRepository, VerifyEmailRepository>();
        services.AddScoped<IVerifyEmailService, VerifyEmail>();
        services.AddScoped<EmailVerificationLinkFactory>();
        services.AddScoped<HttpContextAccessor>();
        services.AddScoped<VerifyEmail>();
        services.AddHttpContextAccessor();
        // services.AddFluentEmail(configuration["EmailUserName"])
        //     .AddSmtpSender(new System.Net.Mail.SmtpClient(configuration["EmailHost"])
        //     {
        //         Port = configuration.GetValue<int>("EmailPort"),
        //         Credentials = new System.Net.NetworkCredential(
        //             configuration["EmailUserName"], 
        //             configuration["EmailPassword"]
        //         ),
        //         EnableSsl = true 
        //     });
        return services;
    }
}