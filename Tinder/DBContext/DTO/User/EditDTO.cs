using System.ComponentModel.DataAnnotations;
using Tinder.DBContext.Models.Enums;

namespace Tinder.DBContext.DTO.User;
public class EditDTO
{
    [MaxLength(50)] public string? FirstName { get; set; }

    [MaxLength(50)] public string? LastName { get; set; }

    public DateTime? BirthDate { get; set; }

    public EducationLevel? EducationLevel { get; set; }

    public int? Course { get; set; }

    [EmailAddress] public string? Email { get; set; }

    [MaxLength(100)] public string? Telegram { get; set; }

    public string? PhotoUrl { get; set; }

    [MaxLength(1000)] public string? About { get; set; }

    public Gender? Gender { get; set; }
}
