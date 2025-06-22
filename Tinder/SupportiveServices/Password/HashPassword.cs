namespace Tinder.SupportiveServices.Password;

public class HashPassword
{
    public string HashingPassword(string password)
    {
        const int workFactor = 12; 
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, workFactor);
        return hashedPassword;
    }

    public static bool VerifyPassword(string providedPassword, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}