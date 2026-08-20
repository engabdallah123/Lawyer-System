using System.Security.Claims;
using App.Infrastructure.Identity.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace App.Web.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private const string PermissionClaimType = "Permission";
    private static ClaimsPrincipal _currentUserPrincipal = new(new ClaimsIdentity());
    private static UserDto? _currentUser;

    public UserDto? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null && _currentUserPrincipal.Identity?.IsAuthenticated == true;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUserPrincipal));
    }

    public void MarkUserAsAuthenticated(UserDto user)
    {
        _currentUser = user;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("JobTitle", user.JobTitle ?? string.Empty)
        };

        foreach (var perm in user.Permissions)
        {
            claims.Add(new Claim(PermissionClaimType, perm));
        }

        var identity = new ClaimsIdentity(claims, "CustomAuth");
        _currentUserPrincipal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUserPrincipal)));
    }

    public void MarkUserAsLoggedOut()
    {
        _currentUser = null;
        _currentUserPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUserPrincipal)));
    }

    public bool HasPermission(string permission)
    {
        if (_currentUser == null)
            return false;

        // Admin has all permissions
        if (_currentUser.Role == "Administrator")
            return true;

        return _currentUser.Permissions.Contains(permission);
    }

    public bool IsInRole(string role)
    {
        return _currentUser?.Role == role;
    }
}
