namespace Tinder.DBContext.DTO;

public class SendMessageRequest
{
    public Guid ToUserId { get; set; }
    public string Content { get; set; } = null!;
}