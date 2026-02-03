using Clean.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clean.Infrastructure.Data;

public class DataContext:DbContext,IDbContext
{
    public DataContext(DbContextOptions<DataContext> options):base(options){}
    public DbSet<User> Users { get; set; }
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
}