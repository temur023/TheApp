using Clean.Application.Filters;
using Clean.Domain.Entities;

namespace Clean.Application.Abstractions;

public interface IUserRepository
{
    Task<(List<User> Users, int Total)> GetAll(UserFilter filter);
    Task<User?> GetById(int id);
    Task Update();
    
    Task<List<User>> DeleteSelected(List<int> ids);
    Task<List<User>> BlockSelected(List<int> ids);
    Task<List<User>> UnblockSelected(List<int> ids);
    Task<List<User>> DeleteUnverified();
}