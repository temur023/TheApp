using Clean.Application.Dtos;
using Clean.Application.Filters;
using Clean.Application.Responses;
using Clean.Domain.Entities;

namespace Clean.Application.Abstractions;

public interface IAuthenticationRepository
{
    
    Task<int> Register(User user, EmailVerificationToken verificationToken);
    Task<User?> Login(string email);
}