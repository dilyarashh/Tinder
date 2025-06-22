using System.Text.RegularExpressions;

namespace Tinder.SupportiveServices.Validations;
public static class NameValidator
{
    public static bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
        {
            return false;
        }

        return Regex.IsMatch(name, @"^[A-ZА-ЯЁ][a-zа-яё]{1,}$");
    }
}