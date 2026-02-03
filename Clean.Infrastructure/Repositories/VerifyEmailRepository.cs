using Clean.Application.Abstractions;
using Clean.Domain.Entities;
using Clean.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clean.Infrastructure.Repositories;

public class VerifyEmailRepository(DataContext context):IVerifyEmailRepository
{
        public async Task<Status> Handle(Guid tokenId)
        {
            var token = await context.EmailVerificationTokens
                .Include(t=>t.User)
                .FirstOrDefaultAsync(t => t.Id == tokenId);
            if (token == null || token.Expires<DateTimeOffset.UtcNow)
            {
                return Status.Unverified;
            }

            if (token.User.Status == Status.Blocked)
            {
                return Status.Blocked;
            }

            token.User.Status = Status.Active;
            context.EmailVerificationTokens.Remove(token);
            await context.SaveChangesAsync();
            return Status.Active;
        }
}