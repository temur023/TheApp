using System.Security.Claims;
using Clean.Application.Abstractions;
using Clean.Application.Dtos;
using Clean.Application.Filters;
using Clean.Application.Responses;
using Clean.Domain.Entities;
using Microsoft.AspNetCore.Http;


namespace Clean.Application.Services;

public class UserService(IUserRepository repository,IHttpContextAccessor httpContextAccessor):IUserService
{
    private int? GetCurrentUserId() => int.TryParse(httpContextAccessor.HttpContext?.User
        .FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId) ? userId : null;
    private async Task<bool> IsActorActive()
    {
        var id = GetCurrentUserId();
        if (id == null) return false;
        var actor = await repository.GetById(id.Value);
        if (actor == null) return false;
        return actor.Status == Status.Active;
    }
    public async Task<PagedResponse<UserGetDto>> GetAll(UserFilter filter)
    {
        if (!await IsActorActive()) 
            return new PagedResponse<UserGetDto>(new List<UserGetDto>(), 0, 0, 0,"Access Denied");
        var users = await repository.GetAll(filter);
        var dto = users.Users.Select(u => new UserGetDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            DateOfRegistration = u.DateOfRegistration,
            LastSeen = getLastSeen(u.LastSeen.ToUniversalTime()),
            Status = u.Status
        }).ToList();
        return new PagedResponse<UserGetDto>(dto, filter.PageNumber, filter.PageSize, users.Total, "200");
    }

    public async Task<Response<UserGetDto>> GetById(int id)
    {
        var find = await repository.GetById(id);
        if (find == null) return new Response<UserGetDto>(404, "User not found!");
        var dto = new UserGetDto()
        {
            Id = find.Id,
            FullName = find.FullName,
            Email = find.Email,
            LastSeen = getLastSeen(find.LastSeen.ToUniversalTime()),
            DateOfRegistration = find.DateOfRegistration,
            Status = find.Status
        };
        return new Response<UserGetDto>(200, "User found", dto);
    }
    

    public async Task<Response<string>> DeleteSelected(List<int> ids)
    {
        if (!await IsActorActive())    
            return new Response<string>(400,"Access Denied");
        var users = await repository.DeleteSelected(ids);
        if (!users.Any()) return new Response<string>(404, "No user found!");
        return new Response<string>(200, $"{users.Count} Users have been deleted!");
    }

    public async Task<Response<string>> BlockSelected(List<int> ids)
    {
        if (!await IsActorActive())   
            return new Response<string>(400,"Access Denied");
        var users = await repository.BlockSelected(ids);
        if (!users.Any()) return new Response<string>(404, "No user found!");
        foreach (var user in users)
        {
            user.prevStatus = user.Status;
            user.Status = Status.Blocked;
        }
        await repository.Update();
        return new Response<string>(200, $"{users.Count} Users have been blocked");
    }

    public async Task<Response<string>> UnblockSelected(List<int> ids)
    {
        if (!await IsActorActive())   
            return new Response<string>(400,"Access Denied");
        var users = await repository.UnblockSelected(ids);
        if (!users.Any()) return new Response<string>(404, "No user found!");
        foreach (var user in users)
        {
            user.Status = user.prevStatus;
        }

        await repository.Update();
        return new Response<string>(200, $"{users.Count} Users have been unblocked");
    }

    public async Task<Response<string>> DeleteUnverified()
    {
        if (!await IsActorActive())     
            return new Response<string>(400,"Access Denied");
        var users = await repository.DeleteUnverified();
        if (!users.Any()) return new Response<string>(404, "No unverified users found!");
        return new Response<string>(200, $"{users.Count} Unverified users have been deleted");
    }

    private string getLastSeen(DateTimeOffset seen)
    {
        long a = (int)((DateTimeOffset.UtcNow - seen).TotalSeconds);
        if (a < 60)
            return $"last seen {a} seconds ago";
        if (a < 3600)
            return $"last seen {a/60} minutes ago";
        if (a < 86400)
            return $"last seen {a/3600} hours ago";
        if (a < 2592000)
            return $"last seen {a / 86400} days ago";
        if (a < 31104000)
            return $"last seen {a / 2592000} months ago";
        return $"last seen {a / 31104000} years ago";
    }
}