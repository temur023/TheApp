using Clean.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Clean.Infrastructure;

public class EmailVerificationLinkFactory(IConfiguration configuration)
{
    public string Create(EmailVerificationToken verificationToken)
    {
        var baseUrl = configuration["AppBaseUrl"];
        return $"{baseUrl.TrimEnd('/')}/users/verify-email?token={verificationToken.Id}";
    }
}