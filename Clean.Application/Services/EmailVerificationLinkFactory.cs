using Clean.Domain.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
public class EmailVerificationLinkFactory(IConfiguration configuration)
{
    public string Create(EmailVerificationToken verificationToken)
    {
        var baseUrl = configuration["AppBaseUrl"]; 
        
        if (string.IsNullOrEmpty(baseUrl))
        {
            return "Base URL not configured!";
        }

        return $"{baseUrl.TrimEnd('/')}/users/verify-email?token={verificationToken.Id}";
    }
}