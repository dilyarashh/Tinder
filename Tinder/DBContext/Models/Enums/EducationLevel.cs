using System.Text.Json.Serialization;

namespace Tinder.DBContext.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EducationLevel
{
    Bachelor,    
    Master    
}