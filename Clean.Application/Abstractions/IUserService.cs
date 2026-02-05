using Clean.Application.Dtos;
using Clean.Application.Filters;
using Clean.Application.Responses;

namespace Clean.Application.Abstractions;

public interface IUserService
{
    Task<PagedResponse<UserGetDto>> GetAll(UserFilter filter);
    Task<Response<UserGetDto>> GetById(int id);
    Task<Response<string>> DeleteSelected(List<int> ids);
    Task<Response<string>> BlockSelected(List<int> ids);
    Task<Response<string>> UnblockSelected(List<int> ids);

    Task<Response<string>> DeleteUnverified();
}