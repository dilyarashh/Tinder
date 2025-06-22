using Tinder.DBContext.Models.Enums;

namespace Tinder.DBContext.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] [MaxLength(50)] public string FirstName { get; set; } = null!;

    [Required] [MaxLength(50)] public string LastName { get; set; } = null!;

    [Required] public DateTime BirthDate { get; set; }

    [Required] public int Age { get; set; }

    [Required] public EducationLevel EducationLevel { get; set; }
    
    [Required] public int Course { get; set; }

    [Required] public string Password { get; set; } = null!; 

    [Required] [EmailAddress] public string Email { get; set; } = null!;

    [Required] public string Telegram { get; set; }

    public string? PhotoUrl { get; set; } 

    public string? About { get; set; }
    
    [Required] public Gender Gender { get; set; }
}