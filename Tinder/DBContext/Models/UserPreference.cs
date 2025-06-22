using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tinder.DBContext.Models;

public class UserPreference
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid FromUserId { get; set; }

    [Required]
    public Guid ToUserId { get; set; }

    [Required]
    public bool IsLiked { get; set; }

    [ForeignKey(nameof(FromUserId))]
    public User FromUser { get; set; } = null!;

    [ForeignKey(nameof(ToUserId))]
    public User ToUser { get; set; } = null!;
}