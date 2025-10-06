using FcmsPortal.Enums;
using FcmsPortal.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Cryptography;

namespace FcmsPortalUI.Components.Account
{
    public static class AccountSeeder
    {
        // Create the special system accounts: Developer and PrincipalBackup
        // config keys used:
        // InitialDeveloper:Email
        // InitialDeveloper:Password
        // InitialPrincipalBackup:Email
        // InitialPrincipalBackup:Password
        public static async Task EnsureSpecialAccountsAsync(IServiceProvider services, IConfiguration config, bool isDevelopment = false)
        {
            var userManager = services.GetRequiredService<UserManager<Person>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // Helper: create or get user, set password, assign role and a SystemAccount claim
            async Task<Person> CreateOrEnsureAccount(string email, string password, string roleName, string displayFirstName, string displayLastName)
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email is required for special account creation.");

                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new Person
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        FirstName = displayFirstName,
                        LastName = displayLastName,
                        IsActive = true,
                        IsArchived = false,
                        DateOfEnrollment = DateTime.UtcNow.Date,
                        DateOfBirth = DateTime.UtcNow.Date.AddYears(-30),
                        StateOfOrigin = "N/A",
                        LgaOfOrigin = "N/A",
                        EmergencyContact = "0000000000",
                        Address = new Address
                        {
                            Street = "System Street",
                            City = "System City",
                            State = "System State",
                            PostalCode = "0000",
                            Country = "System Country"
                        }
                    };


                    var createResult = await userManager.CreateAsync(user, password);
                    if (!createResult.Succeeded)
                    {
                        throw new Exception($"Failed to create system account '{email}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                    }
                }

                // Ensure role exists
                if (await roleManager.FindByNameAsync(roleName) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }

                // Assign role if not already assigned
                if (!await userManager.IsInRoleAsync(user, roleName))
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }

                // Add a claim that marks this as a system account (helps hide it in UIs)
                var claims = await userManager.GetClaimsAsync(user);
                if (!claims.Any(c => c.Type == "SystemAccount" && c.Value == "true"))
                {
                    await userManager.AddClaimAsync(user, new Claim("SystemAccount", "true"));
                }

                return user;
            }

            // Developer account credentials (from config)
            var devEmail = config["InitialDeveloper:Email"] ?? "dev@temp.local";
            var devPassword = config["InitialDeveloper:Password"] ?? "TempP@ssw0rd!";

            // Principal backup account credentials (from config)
            var principalEmail = config["InitialPrincipalBackup:Email"] ?? "prince@temp.local";
            var principalPassword = config["InitialPrincipalBackup:Password"] ?? "TempP@ssw0rd!";

            // If credentials are missing in production, **failsafe** 
            if (string.IsNullOrWhiteSpace(devEmail) || string.IsNullOrWhiteSpace(devPassword))
            {
                if (isDevelopment)
                {
                    devEmail ??= $"dev+{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8)}@system.local";
                    devPassword ??= GenerateStrongPassword();
                    Console.WriteLine($"[Dev system account generated] Email: {devEmail} Password: {devPassword}");
                }
                else
                {
                    throw new InvalidOperationException("Missing InitialDeveloper credentials in configuration. Provide InitialDeveloper:Email and InitialDeveloper:Password.");
                }
            }

            if (string.IsNullOrWhiteSpace(principalEmail) || string.IsNullOrWhiteSpace(principalPassword))
            {
                if (isDevelopment)
                {
                    principalEmail ??= $"principal-backup+{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8)}@system.local";
                    principalPassword ??= GenerateStrongPassword();
                    Console.WriteLine($"[Principal backup generated] Email: {principalEmail} Password: {principalPassword}");
                }
                else
                {
                    throw new InvalidOperationException("Missing InitialPrincipalBackup credentials in configuration. Provide InitialPrincipalBackup:Email and InitialPrincipalBackup:Password.");
                }
            }

            // Create Developer system account
            await CreateOrEnsureAccount(devEmail!, devPassword!, nameof(UserRole.Developer), "System", "Developer");

            // Create Principal backup account
            await CreateOrEnsureAccount(principalEmail!, principalPassword!, nameof(UserRole.Principal), "Backup", "Principal");
        }

        // Basic strong password generator 
        private static string GenerateStrongPassword(int length = 16)
        {
            const string upper = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijkmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*()-_=+";
            var all = upper + lower + digits + special;

            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);

            var chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = all[bytes[i] % all.Length];

            if (length >= 4)
            {
                chars[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
                chars[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
                chars[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
                chars[3] = special[RandomNumberGenerator.GetInt32(special.Length)];
            }

            for (int i = length - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                var tmp = chars[i];
                chars[i] = chars[j];
                chars[j] = tmp;
            }

            return new string(chars);
        }
    }
}
