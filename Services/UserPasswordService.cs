namespace GymTracker.UserService.Services;

public static class UserPasswordService
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public static bool IsValidPassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password) &&
               password.Length >= 6 &&
               password.Length <= 100;
    }
}