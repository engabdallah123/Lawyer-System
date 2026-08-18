using Microsoft.AspNetCore.Identity;

namespace App.Infrastructure.Identity.Models;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }

    public ApplicationRole() : base() { }

    public ApplicationRole(string roleName, string? description = null) : base(roleName)
    {
        Description = description;
    }
}
