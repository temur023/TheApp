using Clean.Domain.Entities;

namespace Clean.Application.Dtos;

public class UserCreateDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string CheckPassword { get; set; } = "";
}