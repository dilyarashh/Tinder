namespace Tinder.DBContext.Models;

public class BlackToken
{
    public Guid Id { get; set; }
    public string? Blacktoken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}