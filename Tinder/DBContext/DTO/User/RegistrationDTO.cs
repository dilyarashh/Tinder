using System.ComponentModel.DataAnnotations;
using Tinder.DBContext.Models.Enums;

namespace Tinder.DBContext.DTO.User;
public class RegistrationDTO
{
    [Required] [MaxLength(50)] public string FirstName { get; set; } = null!;

    [Required] [MaxLength(50)] public string LastName { get; set; } = null!;

    [Required] public DateTime BirthDate { get; set; }

    [Required] public EducationLevel EducationLevel { get; set; }

    [Required] public int Course { get; set; }

    [Required] public string Password { get; set; } = null!;

    [Required] [EmailAddress] public string Email { get; set; } = null!;

    [Required] public string Telegram { get; set; } = null!;

    public string? PhotoUrl { get; set; }

    [MaxLength(1000)] public string? About { get; set; }
    
    [Required] public Gender Gender { get; set; }
}