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

    public UserService(UserDbContext context)
    {
        _context = context;
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

    // Helper method - hashing זמני (נשפר מאוחר יותר)
    private static string HashPassword(string password)
    {
        // TODO: השתמש בBCrypt או Argon2
        return password + "_hashed"; // זמני בלבד!
    }
}   