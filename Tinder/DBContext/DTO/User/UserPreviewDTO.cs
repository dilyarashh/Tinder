using Tinder.DBContext.Models.Enums;

namespace Tinder.DBContext.DTO.User;

public class UserPreviewDTO
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    
    public EducationLevel EducationLevel { get; set; }
    public string? PhotoUrl { get; set; }
    public int Course { get; set; }
    public int Age { get; set; }
    public string? About { get; set; } = null!;

    public string Telegram { get; set; } = null!;
}