using FcmsPortal.Enums;
using FcmsPortal.Models;
using Microsoft.AspNetCore.Identity;

namespace FcmsPortalUI.Components.Account
{
    public static class RoleSeeder
    {
        public static async Task EnsureRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            var roleNames = Enum.GetNames(typeof(UserRole))
                                .Where(n => !string.Equals(n, nameof(UserRole.None), StringComparison.OrdinalIgnoreCase));

            foreach (var roleName in roleNames)
            {
                if (await roleManager.FindByNameAsync(roleName) == null)
                {
                    var result = await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        // create an initial principal if none exists
        public static async Task EnsureAdminAsync(IServiceProvider serviceProvider, string principalEmail, string principalPassword)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Person>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // ensure Principal role exists
            if (await roleManager.FindByNameAsync(nameof(UserRole.Principal)) == null)
                await roleManager.CreateAsync(new IdentityRole<int>(nameof(UserRole.Principal)));

            var principal = await userManager.FindByEmailAsync(principalEmail);
            if (principal == null)
            {
                principal = new Person
                {
                    UserName = principalEmail,
                    Email = principalEmail,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator"
                };

                var createResult = await userManager.CreateAsync(principal, principalPassword);
                if (!createResult.Succeeded)
                {
                    throw new Exception("Failed to create admin user: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(principal, nameof(UserRole.Principal)))
            {
                await userManager.AddToRoleAsync(principal, nameof(UserRole.Principal));
            }
        }
    }
}
