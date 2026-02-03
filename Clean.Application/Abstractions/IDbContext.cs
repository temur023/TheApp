using Clean.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public interface IDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}