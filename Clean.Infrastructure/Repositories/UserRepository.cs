using Clean.Application.Abstractions;
using Clean.Application.Filters;
using Clean.Domain.Entities;
using Clean.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clean.Infrastructure.Repositories;

public class UserRepository(DataContext context):IUserRepository
{
    
    public async Task<(List<User> Users, int Total)> GetAll(UserFilter filter)
    {
        
        var query = context.Users.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(u => u.FullName.Contains(filter.Name));
        if (filter.Status.HasValue)
            query = query.Where(u => u.Status == filter.Status);
        var total = await query.CountAsync();
        var users = await query
            .OrderByDescending(x => x.LastSeen) 
            .Skip((filter.PageNumber - 1) * filter.PageSize) 
            .Take(filter.PageSize)
            .ToListAsync();
        return (users, total);
    }

    public async Task<User?> GetById(int id)
    {
        var find = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        return find;
    }
    
    public async Task Update()
    {
        await context.SaveChangesAsync();
    }

    public async Task<List<User>> DeleteSelected(List<int> ids)
    {
        if (!ids.Any()) return new List<User>();
        var users = await context.Users.Where(u => ids.Contains(u.Id))
            .ToListAsync();
        if (users.Any())
        {
            context.Users.RemoveRange(users);
            await context.SaveChangesAsync();  
        }
        return users;
    }

    public async Task<List<User>> BlockSelected(List<int> ids)
    {
        if (!ids.Any()) return new List<User>();
        var users = await context.Users.Where(u => ids.Contains(u.Id))
            .ToListAsync();
        return users;
    }

    public async Task<List<User>> UnblockSelected(List<int> ids)
    {
       
        if (!ids.Any()) return new List<User>();
        var users = await context.Users.Where(u => ids.Contains(u.Id))
            .ToListAsync();
        return users;
    }

    public async Task<List<User>> DeleteUnverified()
    {
        var find = await context.Users.Where(u => u.Status == Status.Unverified)
            .ToListAsync();
        if (find.Any())
        {
            context.Users.RemoveRange(find);
            await context.SaveChangesAsync();
        }
        return find;
    }
}