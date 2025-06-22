using Tinder.DBContext.Models.Enums;

namespace Tinder.SupportiveServices.Validations;

public static class CourseValidator
{
    public static bool IsValidCourse(int course, EducationLevel educationLevel)
    {
        return educationLevel switch
        {
            EducationLevel.Bachelor => course is >= 1 and <= 4,
            EducationLevel.Master => course is >= 1 and <= 2,
            _ => false
        };
    }
}