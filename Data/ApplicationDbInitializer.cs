using ASP_Fund_Project.Models;
using Microsoft.AspNetCore.Identity;

namespace ASP_Fund_Project.Data;

public static class ApplicationDbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync(ApplicationRoles.Administrator))
        {
            await roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Administrator));
        }

        if (!await roleManager.RoleExistsAsync(ApplicationRoles.Supporter))
        {
            await roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Supporter));
        }

        const string adminEmail = "admin@communityfundhub.local";
        const string adminPassword = "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Platform Administrator"
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException("Default admin user could not be created.");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, ApplicationRoles.Administrator))
        {
            await userManager.AddToRoleAsync(adminUser, ApplicationRoles.Administrator);
        }
    }
}
