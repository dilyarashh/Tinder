namespace Tinder.DBContext.DTO.User;

public class ReactionListDTO
{
    public int Count { get; set; }
    public List<UserPreviewDTO> Users { get; set; } = new();
}
