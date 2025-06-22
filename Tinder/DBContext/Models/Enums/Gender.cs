using System.Text.Json.Serialization;

namespace Tinder.DBContext.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Gender
{
    Male,
    Female
}