using Clean.Application.Abstractions;
using Clean.Domain.Entities;
using Clean.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clean.Infrastructure.Repositories;
public class AuthenticationRepository(DataContext context):IAuthenticationRepository
{ 
    public async Task<int> Register(User user, EmailVerificationToken verificationToken)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync(); 
        
        verificationToken.UserId = user.Id; 
 
        await context.EmailVerificationTokens.AddAsync(verificationToken);
        await context.SaveChangesAsync();
    
        return user.Id;
    }
    
    
    public async Task<User?> Login(string email)
    {
        var find = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return find;
    }
}