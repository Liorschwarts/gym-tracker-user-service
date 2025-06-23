namespace GymTracker.UserService.Services;

public static class UserPasswordService
{
    public static string HashPassword(string password)
    {
        ValidatePassword(password);
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public static bool IsValidPassword(string password) =>
        !string.IsNullOrWhiteSpace(password) &&
        password.Length >= 6 &&
        password.Length <= 100;

    private static void ValidatePassword(string password)
    {
        if (!IsValidPassword(password))
            throw new ArgumentException("Password does not meet requirements");
    }
}