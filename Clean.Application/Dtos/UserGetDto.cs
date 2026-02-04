using Clean.Domain.Entities;

namespace Clean.Application.Dtos;

public class UserGetDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string LastSeen { get; set; }
    public DateTimeOffset DateOfRegistration { get; set; }
    public Status Status { get; set; }
}