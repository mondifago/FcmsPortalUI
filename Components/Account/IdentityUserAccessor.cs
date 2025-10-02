using FcmsPortal.Models;
using Microsoft.AspNetCore.Identity;

namespace FcmsPortalUI.Components.Account
{
    internal sealed class IdentityUserAccessor(UserManager<Person> userManager, IdentityRedirectManager redirectManager)
    {
        public async Task<Person> GetRequiredUserAsync(HttpContext context)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
            }

            return user;
        }
    }
}
