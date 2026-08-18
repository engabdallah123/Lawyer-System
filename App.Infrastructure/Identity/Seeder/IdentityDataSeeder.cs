using System.Security.Claims;
using App.Domain.Lookups;
using App.Domain.Users.Constants;
using App.Infrastructure.Database;
using App.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        var dbContext = scope.ServiceProvider.GetRequiredService<LegalPracticeDbContext>();
        var logger = scope.ServiceProvider.GetService<ILogger<ApplicationUser>>();

        // 0. Seed Egyptian Lookup Data (CaseTypes, CaseStatuses, Courts, DocumentTypes)
        await SeedLookupsAsync(dbContext, logger);

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
                PhoneNumber = "01000000001",
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
                FullName = "أ. أحمد فتحي المحامي",
                JobTitle = "محامي بالاستئناف العالي ومجلس الدولة",
                PhoneNumber = "01000000002",
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
                FullName = "محمود السيد",
                JobTitle = "محتضر وإداري تنفيذ ومتابعة",
                PhoneNumber = "01000000003",
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

    private static async Task SeedLookupsAsync(LegalPracticeDbContext dbContext, ILogger? logger)
    {
        // Refresh lookups if old generic names exist
        if (await dbContext.Courts.AnyAsync(c => c.Name.Contains("المحكمة العامة")))
        {
            dbContext.Courts.RemoveRange(await dbContext.Courts.ToListAsync());
            dbContext.CaseTypes.RemoveRange(await dbContext.CaseTypes.ToListAsync());
            dbContext.CaseStatuses.RemoveRange(await dbContext.CaseStatuses.ToListAsync());
            dbContext.DocumentTypes.RemoveRange(await dbContext.DocumentTypes.ToListAsync());
            await dbContext.SaveChangesAsync();
        }

        bool hasChanges = false;

        if (!await dbContext.CaseTypes.AnyAsync())
        {
            dbContext.CaseTypes.AddRange(
                new CaseType("تجاري واقتصادي", "الدعاوى والأوراق التجارية واستثمار الشركات"),
                new CaseType("مدني وتعويضات", "الدعاوى والنزاعات المدنية والالتزامات والحقوق"),
                new CaseType("عمالي وتأمينات", "منازعات العمل والعمال ومستحقات التأمينات الاجتماعي"),
                new CaseType("جنح وجنايات", "القضايا الجنائية والجنح والعرائض بالمحاكم"),
                new CaseType("قضاء إداري (مجلس الدولة)", "الطعون والمظالم والقرارات الإدارية الحكومية"),
                new CaseType("أسرة وأحوال شخصية", "دعاوى الأسرة والتركات والنفقة والوراثة"),
                new CaseType("عقاري وإيجارات", "الملكية والنزاعات العقارية وعقود الإيجار طرد وتأخير"),
                new CaseType("تنفيذ وإشكالات", "إشكالات وسندات وقضايا التنفيذ بمحاكم التنفيذ")
            );
            hasChanges = true;
        }

        if (!await dbContext.CaseStatuses.AnyAsync())
        {
            dbContext.CaseStatuses.AddRange(
                new CaseStatus("مفتوحة", "#10b981"),
                new CaseStatus("منظورة بالمحكمة", "#2563eb"),
                new CaseStatus("معلقة", "#f59e0b"),
                new CaseStatus("محكوم فيها", "#8b5cf6"),
                new CaseStatus("استئناف / نقض", "#ec4899"),
                new CaseStatus("مغلقة ومستوفاة", "#64748b")
            );
            hasChanges = true;
        }

        if (!await dbContext.Courts.AnyAsync())
        {
            dbContext.Courts.AddRange(
                new Court("المحكمة الاقتصادية", "القاهرة"),
                new Court("محكمة مجلس الدولة (القضاء الإداري)", "القاهرة"),
                new Court("محكمة النقض", "القاهرة"),
                new Court("محكمة استئناف القاهرة", "القاهرة"),
                new Court("محكمة الأسرة (مجمع المحاكم)", "القاهرة"),
                new Court("المحكمة الابتدائية (مجمع المحاكم)", "الجيزة / القاهرة"),
                new Court("محكمة الجُنح والمخالفات الجزئية", "القاهرة"),
                new Court("محكمة الجنايات", "القاهرة"),
                new Court("محكمة الأمور المستعجلة", "القاهرة"),
                new Court("محكمة استئناف الإسكندرية", "الإسكندرية")
            );
            hasChanges = true;
        }

        if (!await dbContext.DocumentTypes.AnyAsync())
        {
            dbContext.DocumentTypes.AddRange(
                new DocumentType("صحيفة دعوى", "صحائف الدعوى والمذكرات الافتتاحية"),
                new DocumentType("مذكرة دفاع / جوابية", "المذكرات الجوابية والردود القانونية"),
                new DocumentType("عقد / اتفاقية", "العقود والاتفاقيات والبروتوكولات"),
                new DocumentType("توكيل رسمي عام / خاص", "التوكيلات الرسمية بالشهر العقاري"),
                new DocumentType("حكم قضائي / صيغة تنفيذية", "الأحكام والصيغ التنفيذية"),
                new DocumentType("إنذار / صك رسمي", "الإنذارات الرسمية على يد محضر والصكوك"),
                new DocumentType("تقرير خبير / مراجع", "تقارير الخبراء والمراجعين بالمحكمة"),
                new DocumentType("حافظة مستندات / أدلة", "حافظة المستندات والبينات المؤيدة")
            );
            hasChanges = true;
        }

        if (hasChanges)
        {
            await dbContext.SaveChangesAsync();
            logger?.LogInformation("Egyptian Lookup tables (CaseTypes, CaseStatuses, Courts, DocumentTypes) seeded successfully.");
        }
    }
}
