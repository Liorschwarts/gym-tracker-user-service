using GymTracker.UserService.Data;
using GymTracker.UserService.Models;
using GymTracker.UserService.DTOs;
using GymTracker.UserService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.UserService.Services;

// Implementation של IUserService
public class UserService : IUserService
{
    private readonly UserDbContext _context;
    private readonly IJwtService _jwtService;  

    public UserService(UserDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService; 
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .Where(u => u.IsActive)
            .ToListAsync();

        return users.Select(ToResponseDto).ToList();
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

        return user == null ? null : ToResponseDto(user);
    }

    public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        return user == null ? null : ToResponseDto(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = createUserDto.Email,
            PasswordHash = HashPassword(createUserDto.Password), // נוסיף hashing מאוחר יותר
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return ToResponseDto(user);
    }

    public async Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

        if (user == null) return null;

        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ToResponseDto(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

        if (user == null) return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        // חפש משתמש לפי אימייל
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive);

        // אם אין משתמש או סיסמה שגויה
        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            return null;  // Login failed
        }

        // המר לDTO
        var userDto = ToResponseDto(user);

        // יצור JWT token אמיתי
        var token = _jwtService.GenerateToken(userDto);

        return new LoginResponseDto
        {
            Token = token,  // ← Token אמיתי!
            User = userDto,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    // Helper method - המרה מUser לUserResponseDto
    private static UserResponseDto ToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        };
    }

    private static string HashPassword(string password)
    {
        // BCrypt עם cost factor 12 (בטוח אבל לא איטי מדי)
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}   