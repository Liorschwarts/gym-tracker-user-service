using Microsoft.AspNetCore.Mvc;
using GymTracker.UserService.Models;
using GymTracker.UserService.Services;

namespace GymTracker.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    // Dependency Injection - קבלת UserService במקום DbContext ישירות
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // קבלת כל המשתמשים
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // קבלת משתמש לפי ID
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound($"User with ID {id} not found");

        return Ok(user);
    }

    // יצירת משתמש חדש
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        var createdUser = await _userService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
    }

    // עדכון משתמש
    [HttpPut("{id}")]
    public async Task<ActionResult<User>> UpdateUser(Guid id, User user)
    {
        var updatedUser = await _userService.UpdateUserAsync(id, user);
        if (updatedUser == null)
            return NotFound($"User with ID {id} not found");

        return Ok(updatedUser);
    }

    // מחיקת משתמש
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        if (!deleted)
            return NotFound($"User with ID {id} not found");

        return NoContent(); // 204 - מחיקה בהצלחה
    }

    [HttpGet("health")]
    public ActionResult<string> HealthCheck()
    {
        return Ok("User Service is healthy");
    }
}