using GymTracker.UserService.DTOs;

namespace GymTracker.UserService.Interfaces;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(Guid id);
    Task<UserResponseDto?> GetUserByEmailAsync(string email);
    Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserResponseDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto); 
    Task<bool> DeleteUserAsync(Guid id);
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
}