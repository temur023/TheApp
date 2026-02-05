using System.ComponentModel.DataAnnotations;

namespace Clean.Domain.Entities;

public class User
{
    public int Id { get; set; }
    [Required]
    [MaxLength(60)]
    public string FullName { get; set; } = "";
    [Required]
    public string Email { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";
    public DateTimeOffset LastSeen { get; set; }
    public DateTimeOffset DateOfRegistration { get; set; }
    public Status prevStatus { get; set; }
    
    public Status Status { get; set; }
}