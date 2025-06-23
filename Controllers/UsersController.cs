using Microsoft.AspNetCore.Mvc;
using GymTracker.UserService.DTOs;
using GymTracker.UserService.Interfaces;

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
    public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
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
    public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto createUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userService.CreateUserAsync(createUserDto);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    /// <summary>
    /// Update existing user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(Guid id, UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userService.UpdateUserAsync(id, updateUserDto);
        if (user == null)
            return NotFound($"User with ID {id} not found");

        return Ok(user);
    }

    /// <summary>
    /// Delete user (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        if (!deleted)
            return NotFound($"User with ID {id} not found");

        return NoContent();
    }

    /// <summary>
    /// User login with email and password
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.LoginAsync(loginDto);

        if (result == null)
            return Unauthorized("Invalid email or password");

        return Ok(result);
    }

    [HttpGet("health")]
    public ActionResult<string> HealthCheck()
    {
        return Ok("User Service is healthy");
    }
}