namespace Clean.Domain.Entities;

public class EmailVerificationToken
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Expires { get; set; }
}