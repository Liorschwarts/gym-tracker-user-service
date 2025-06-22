using System.ComponentModel.DataAnnotations;

namespace GymTracker.UserService.DTOs;

// DTO לעדכון משתמש - רק שדות שמותר לעדכן
public class UpdateUserDto
{
    [Required(ErrorMessage = "First name is required")]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;
}