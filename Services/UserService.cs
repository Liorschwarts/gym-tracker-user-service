using GymTracker.UserService.Data;
using GymTracker.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.UserService.Services;

// Interface - חוזה של מה הService יעשה
public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<User?> UpdateUserAsync(Guid id, User user);
    Task<bool> DeleteUserAsync(Guid id);
}

// Implementation - איך בפועל הService עובד
public class UserService : IUserService
{
    private readonly UserDbContext _context;

    // Dependency Injection - קבלת הDbContext
    public UserService(UserDbContext context)
    {
        _context = context;
    }

    // קבלת כל המשתמשים
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Where(u => u.IsActive)  // רק משתמשים פעילים
            .ToListAsync();
    }

    // קבלת משתמש לפי ID
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    // קבלת משתמש לפי Email (לlogin)
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    // יצירת משתמש חדש
    public async Task<User> CreateUserAsync(User user)
    {
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    // עדכון משתמש
    public async Task<User?> UpdateUserAsync(Guid id, User user)
    {
        var existingUser = await GetUserByIdAsync(id);
        if (existingUser == null) return null;

        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingUser;
    }

    // מחיקת משתמש (Soft Delete)
    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await GetUserByIdAsync(id);
        if (user == null) return false;

        user.IsActive = false;  // לא באמת מוחק, רק מסמן כלא פעיל
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}