using GymTracker.UserService.Data;
using GymTracker.UserService.Models;
using GymTracker.UserService.DTOs;
using GymTracker.UserService.Interfaces;
using GymTracker.UserService.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.UserService.Services;

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
            .OrderBy(u => u.CreatedAt)
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
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant() && u.IsActive);

        return user == null ? null : ToResponseDto(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var normalizedEmail = createUserDto.Email.ToLowerInvariant();

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (existingUser != null)
            throw new UserAlreadyExistsException(createUserDto.Email);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = UserPasswordService.HashPassword(createUserDto.Password),
            FirstName = createUserDto.FirstName.Trim(),
            LastName = createUserDto.LastName.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return ToResponseDto(user);
    }

    public async Task<UserResponseDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

        if (user == null)
            throw new UserNotFoundException(id);

        // Check email uniqueness if email is being updated
        if (!string.IsNullOrEmpty(updateUserDto.Email))
        {
            var normalizedEmail = updateUserDto.Email.ToLowerInvariant();
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == normalizedEmail && u.Id != id);

            if (existingUser != null)
                throw new UserAlreadyExistsException(updateUserDto.Email);

            user.Email = normalizedEmail;
        }

        user.FirstName = updateUserDto.FirstName.Trim();
        user.LastName = updateUserDto.LastName.Trim();
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
        var normalizedEmail = loginDto.Email.ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail && u.IsActive);

        if (user == null || !UserPasswordService.VerifyPassword(loginDto.Password, user.PasswordHash))
            return null;

        var userDto = ToResponseDto(user);
        var token = _jwtService.GenerateToken(userDto);

        return new LoginResponseDto
        {
            Token = token,
            User = userDto,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    private static UserResponseDto ToResponseDto(User user) =>
        new()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        };
}