using System.Security.Claims;
using App.Domain.Users.Constants;
using App.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.Identity.Seeder;

public static class IdentityDataSeeder
{
    private const string PermissionClaimType = "Permission";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var logger = scope.ServiceProvider.GetService<ILogger<ApplicationUser>>();

        // 1. Seed Roles
        foreach (var roleName in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole(roleName, AppRoles.GetArabicName(roleName));
                await roleManager.CreateAsync(role);
            }
        }

        // 2. Seed Default Administrator Account
        var adminEmail = "admin@lawyer.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "مدير النظام",
                JobTitle = "مدير النظام العام والشركاء",
                PhoneNumber = "0500000001",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SystemSeeder"
            };

            var createResult = await userManager.CreateAsync(adminUser, "Admin@123456");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, AppRoles.Administrator);

                // Add all permissions to Admin
                var allPermissions = AppPermissions.GetAllPermissions();
                foreach (var perm in allPermissions)
                {
                    await userManager.AddClaimAsync(adminUser, new Claim(PermissionClaimType, perm));
                }
                logger?.LogInformation("Admin user seeded successfully ({Email})", adminEmail);
            }
            else
            {
                logger?.LogError("Failed to seed admin user: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }

        // 3. Seed Default Lawyer Account
        var lawyerEmail = "lawyer@lawyer.com";
        var lawyerUser = await userManager.FindByEmailAsync(lawyerEmail);
        if (lawyerUser == null)
        {
            lawyerUser = new ApplicationUser
            {
                UserName = lawyerEmail,
                Email = lawyerEmail,
                EmailConfirmed = true,
                FullName = "أ. خالد الشمري",
                JobTitle = "محامي ومستشار قانوني",
                PhoneNumber = "0500000002",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SystemSeeder"
            };

            var createResult = await userManager.CreateAsync(lawyerUser, "Lawyer@123456");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(lawyerUser, AppRoles.Lawyer);

                var lawyerPermissions = AppPermissions.GetDefaultPermissionsForRole(AppRoles.Lawyer);
                foreach (var perm in lawyerPermissions)
                {
                    await userManager.AddClaimAsync(lawyerUser, new Claim(PermissionClaimType, perm));
                }
                logger?.LogInformation("Lawyer user seeded successfully ({Email})", lawyerEmail);
            }
        }

        // 4. Seed Default Staff Account
        var staffEmail = "staff@lawyer.com";
        var staffUser = await userManager.FindByEmailAsync(staffEmail);
        if (staffUser == null)
        {
            staffUser = new ApplicationUser
            {
                UserName = staffEmail,
                Email = staffEmail,
                EmailConfirmed = true,
                FullName = "أحمد المنصوري",
                JobTitle = "موظف إداري وسكرتارية",
                PhoneNumber = "0500000003",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SystemSeeder"
            };

            var createResult = await userManager.CreateAsync(staffUser, "Staff@123456");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(staffUser, AppRoles.Staff);

                var staffPermissions = AppPermissions.GetDefaultPermissionsForRole(AppRoles.Staff);
                foreach (var perm in staffPermissions)
                {
                    await userManager.AddClaimAsync(staffUser, new Claim(PermissionClaimType, perm));
                }
                logger?.LogInformation("Staff user seeded successfully ({Email})", staffEmail);
            }
        }
    }
}
