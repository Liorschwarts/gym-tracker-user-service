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
    private readonly ILogger<UserService> _logger;

    public UserService(UserDbContext context, IJwtService jwtService, ILogger<UserService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
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
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == createUserDto.Email);

        if (existingUser != null)
            throw new UserAlreadyExistsException(createUserDto.Email);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = createUserDto.Email,
            PasswordHash = HashPassword(createUserDto.Password),
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
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive);

        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
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

    private static string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, 12);

    private static bool VerifyPassword(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}