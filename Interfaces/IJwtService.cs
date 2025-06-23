using GymTracker.UserService.DTOs;

namespace GymTracker.UserService.Interfaces;

/// <summary>
/// Service for JWT token operations
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generate JWT token for user
    /// </summary>
    string GenerateToken(UserResponseDto user);

    /// <summary>
    /// Validate JWT token
    /// </summary>
    bool ValidateToken(string token);

    /// <summary>
    /// Get user ID from token
    /// </summary>
    Guid? GetUserIdFromToken(string token);
}