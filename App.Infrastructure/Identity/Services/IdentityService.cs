using System.Security.Claims;
using App.Domain.Users.Constants;
using App.Infrastructure.Identity.DTOs;
using App.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace App.Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private const string PermissionClaimType = "Permission";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result<UserDto>> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result<UserDto>.Failure(new Error("Auth.InvalidCredentials", "البريد الإلكتروني أو كلمة المرور غير صحيحة"));
        }

        if (!user.IsActive)
        {
            return Result<UserDto>.Failure(new Error("Auth.InactiveUser", "هذا الحساب معطل حالياً، يرجى التواصل مع مدير النظام"));
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
        {
            return Result<UserDto>.Failure(new Error("Auth.InvalidCredentials", "البريد الإلكتروني أو كلمة المرور غير صحيحة"));
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        var permissions = claims.Where(c => c.Type == PermissionClaimType).Select(c => c.Value).ToList();

        var primaryRole = roles.FirstOrDefault() ?? AppRoles.Staff;

        // إذا لم يكن لديه صلاحيات خاصة مخزنة كـ Claims، نعتمد صلاحيات الدور الافتراضية
        if (permissions.Count == 0)
        {
            permissions = AppPermissions.GetDefaultPermissionsForRole(primaryRole);
        }

        var dto = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            JobTitle = user.JobTitle,
            Role = primaryRole,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Permissions = permissions
        };

        return Result<UserDto>.Success(dto);
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var users = await _userManager.Users.AsNoTracking().OrderByDescending(u => u.CreatedAt).ToListAsync();
        var dtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var permissions = claims.Where(c => c.Type == PermissionClaimType).Select(c => c.Value).ToList();

            var primaryRole = roles.FirstOrDefault() ?? AppRoles.Staff;
            if (permissions.Count == 0)
            {
                permissions = AppPermissions.GetDefaultPermissionsForRole(primaryRole);
            }

            dtos.Add(new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                JobTitle = user.JobTitle,
                Role = primaryRole,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Permissions = permissions
            });
        }

        return dtos;
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        var permissions = claims.Where(c => c.Type == PermissionClaimType).Select(c => c.Value).ToList();

        var primaryRole = roles.FirstOrDefault() ?? AppRoles.Staff;
        if (permissions.Count == 0)
        {
            permissions = AppPermissions.GetDefaultPermissionsForRole(primaryRole);
        }

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            JobTitle = user.JobTitle,
            Role = primaryRole,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Permissions = permissions
        };
    }

    public async Task<Result<string>> CreateUserAsync(CreateUserRequest request, string? createdBy = null)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<string>.Failure(new Error("User.EmailExists", "البريد الإلكتروني مستخدم بالفعل"));
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            JobTitle = request.JobTitle,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("، ", result.Errors.Select(e => e.Description));
            return Result<string>.Failure(new Error("User.CreateFailed", $"فشل إنشاء المستخدم: {errors}"));
        }

        // إسناد الدور
        var role = string.IsNullOrWhiteSpace(request.Role) ? AppRoles.Staff : request.Role;
        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new ApplicationRole(role));
        }
        await _userManager.AddToRoleAsync(user, role);

        // إسناد الصلاحيات المخصصة
        var permissions = request.Permissions.Count > 0
            ? request.Permissions
            : AppPermissions.GetDefaultPermissionsForRole(role);

        foreach (var perm in permissions.Distinct())
        {
            await _userManager.AddClaimAsync(user, new Claim(PermissionClaimType, perm));
        }

        return Result<string>.Success(user.Id);
    }

    public async Task<Result> UpdateUserAsync(UpdateUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            return Result.Failure(new Error("User.NotFound", "المستخدم غير موجود"));
        }

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.JobTitle = request.JobTitle;
        user.IsActive = request.IsActive;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join("، ", updateResult.Errors.Select(e => e.Description));
            return Result.Failure(new Error("User.UpdateFailed", $"فشل تحديث البيانات: {errors}"));
        }

        // تحديث الدور
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(request.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!await _roleManager.RoleExistsAsync(request.Role))
                {
                    await _roleManager.CreateAsync(new ApplicationRole(request.Role));
                }
                await _userManager.AddToRoleAsync(user, request.Role);
            }
        }

        // تحديث الصلاحيات
        var currentClaims = await _userManager.GetClaimsAsync(user);
        var currentPermissions = currentClaims.Where(c => c.Type == PermissionClaimType).ToList();

        foreach (var claim in currentPermissions)
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }

        var newPermissions = request.Permissions.Count > 0
            ? request.Permissions
            : AppPermissions.GetDefaultPermissionsForRole(request.Role);

        foreach (var perm in newPermissions.Distinct())
        {
            await _userManager.AddClaimAsync(user, new Claim(PermissionClaimType, perm));
        }

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure(new Error("User.NotFound", "المستخدم غير موجود"));
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("، ", result.Errors.Select(e => e.Description));
            return Result.Failure(new Error("User.PasswordChangeFailed", $"فشل تغيير كلمة المرور: {errors}"));
        }

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure(new Error("User.NotFound", "المستخدم غير موجود"));
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("، ", result.Errors.Select(e => e.Description));
            return Result.Failure(new Error("User.PasswordResetFailed", $"فشل إعادة تعيين كلمة المرور: {errors}"));
        }

        return Result.Success();
    }

    public async Task<Result> ToggleUserStatusAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure(new Error("User.NotFound", "المستخدم غير موجود"));
        }

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);
        return Result.Success();
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure(new Error("User.NotFound", "المستخدم غير موجود"));
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("، ", result.Errors.Select(e => e.Description));
            return Result.Failure(new Error("User.DeleteFailed", $"فشل حذف المستخدم: {errors}"));
        }

        return Result.Success();
    }

    public async Task<List<string>> GetUserPermissionsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return [];

        var claims = await _userManager.GetClaimsAsync(user);
        var perms = claims.Where(c => c.Type == PermissionClaimType).Select(c => c.Value).ToList();

        if (perms.Count == 0)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? AppRoles.Staff;
            perms = AppPermissions.GetDefaultPermissionsForRole(role);
        }

        return perms;
    }
}
