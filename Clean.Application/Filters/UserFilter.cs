using Clean.Domain.Entities;

namespace Clean.Application.Filters;

public class UserFilter
{
    public string? Name { get; set; }
    public Status? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}