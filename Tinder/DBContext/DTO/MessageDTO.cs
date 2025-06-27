namespace Tinder.DBContext.DTO;

public class MessageDTO
{
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime SentAt { get; set; }
}
