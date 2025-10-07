using FcmsPortal.Models;
using Microsoft.AspNetCore.Identity;

namespace FcmsPortalUI.Services
{
    public static class SetupService
    {
        public static async Task EnsurePrincipalExistsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Person>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // Ensure the Principal role exists
            if (await roleManager.FindByNameAsync("Principal") == null)
            {
                await roleManager.CreateAsync(new IdentityRole<int>("Principal"));
            }

            // Get all users in Principal role
            var allPrincipals = await userManager.GetUsersInRoleAsync("Principal");
            var nonSystemPrincipals = new List<Person>();

            foreach (var principal in allPrincipals)
            {
                var claims = await userManager.GetClaimsAsync(principal);
                bool isSystemAccount = claims.Any(c => c.Type == "SystemAccount" && c.Value == "true");

                if (!isSystemAccount)
                {
                    nonSystemPrincipals.Add(principal);
                }
            }

            // If no non-system principal exists, flag setup requirement
            if (nonSystemPrincipals.Count == 0)
            {
                var appState = scope.ServiceProvider.GetRequiredService<AppStateService>();
                appState.NeedPrincipalSetup = true;
            }
        }
    }
}
