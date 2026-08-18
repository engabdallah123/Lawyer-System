using App.Infrastructure.Identity.DTOs;
using Shared.Domain;

namespace App.Infrastructure.Identity.Services;

public interface IIdentityService
{
    Task<Result<UserDto>> AuthenticateAsync(string email, string password);
    Task<List<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<Result<string>> CreateUserAsync(CreateUserRequest request, string? createdBy = null);
    Task<Result> UpdateUserAsync(UpdateUserRequest request);
    Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<Result> ResetPasswordAsync(string userId, string newPassword);
    Task<Result> ToggleUserStatusAsync(string userId);
    Task<Result> DeleteUserAsync(string userId);
    Task<List<string>> GetUserPermissionsAsync(string userId);
}
