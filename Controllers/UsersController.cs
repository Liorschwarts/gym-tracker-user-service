using GymTracker.UserService.Attributes;
using GymTracker.UserService.DTOs;
using GymTracker.UserService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GymTracker.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get all active users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserResponseDto>), 200)]
    public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserResponseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserResponseDto>> GetUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound($"User with ID {id} not found");

        return Ok(user);
    }

    /// <summary>
    /// Create new user
    /// </summary>
    [HttpPost]
    [ValidateModel]
    [ProducesResponseType(typeof(UserResponseDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto createUserDto)
    {
        var user = await _userService.CreateUserAsync(createUserDto);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    /// <summary>
    /// Update existing user
    /// </summary>
    [HttpPut("{id}")]
    [ValidateModel]
    [ProducesResponseType(typeof(UserResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _userService.UpdateUserAsync(id, updateUserDto);
        return Ok(user);
    }

    /// <summary>
    /// Delete user (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        if (!deleted)
            return NotFound($"User with ID {id} not found");

        return NoContent();
    }

    /// <summary>
    /// User login
    /// </summary>
    [HttpPost("login")]
    [EnableRateLimiting("LoginPolicy")]
    [ValidateModel]
    [ProducesResponseType(typeof(LoginResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
    {
        var result = await _userService.LoginAsync(loginDto);
        if (result == null)
            return Unauthorized("Invalid email or password");

        return Ok(result);
    }

    /// <summary>
    /// Health check
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), 200)]
    public ActionResult<object> HealthCheck()
    {
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow, Service = "UserService" });
    }
}