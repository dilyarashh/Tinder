using System.Text.RegularExpressions;

namespace Tinder.SupportiveServices.Validations;

public static class TelegramValidator
{
    public static bool IsValidTelegram(string telegram)
    {
        if (string.IsNullOrWhiteSpace(telegram))
            return false;

        if (telegram.StartsWith("@"))
            telegram = telegram[1..];

        return Regex.IsMatch(telegram, @"^[a-zA-Z0-9_]{5,32}$");
    }
}