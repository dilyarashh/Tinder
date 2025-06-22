using Tinder.DBContext.Models.Enums;

namespace Tinder.DBContext.DTO;

public class FilterDTO
{
    public EducationLevel? EducationLevel { get; set; }
    public int? Course { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public Gender? Gender { get; set; }
}