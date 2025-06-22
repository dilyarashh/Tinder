using Tinder.DBContext.Models.Enums;

namespace Tinder.DBContext.DTO.User;

public class UserDTO
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public int Age { get; set; }

    public EducationLevel EducationLevel { get; set; }

    public int Course { get; set; }

    public string Email { get; set; } = null!;

    public string Telegram { get; set; } = null!;

    public string? PhotoUrl { get; set; }

    public string? About { get; set; }
    public Gender Gender { get; set; }
}