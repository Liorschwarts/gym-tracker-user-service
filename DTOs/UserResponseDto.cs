namespace GymTracker.UserService.DTOs;

// DTO להחזרת מידע משתמש - רק מה שבטוח לחשוף
public class UserResponseDto
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    // שדה מחושב - לא קיים בDB
    public string FullName => $"{FirstName} {LastName}".Trim();

    public DateTime CreatedAt { get; set; }

    // אין: PasswordHash, UpdatedAt, IsActive - לא בטוח לחשוף!
}