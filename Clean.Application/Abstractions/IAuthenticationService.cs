using Clean.Application.Dtos;
using Clean.Application.Filters;
using Clean.Application.Responses;
using Clean.Domain.Entities;

namespace Clean.Application.Abstractions;

public interface IAuthenticationService
{
    
    Task<Response<string>> Register(UserCreateDto user);
    Task<Response<string>> Login(UserLoginDto user);
}