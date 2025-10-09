using FcmsPortal.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FcmsPortalUI.Infrastructure.Startup
{
    public static class AccountEndpoints
    {
        public static IEndpointRouteBuilder MapAccountServices(this IEndpointRouteBuilder app)
        {
            // LOGIN endpoint (optional – for custom redirects)
            app.MapGet("/login", async (HttpContext context, string? returnUrl) =>
            {
                returnUrl ??= "/";
                await context.ChallengeAsync(IdentityConstants.ApplicationScheme,
                    new AuthenticationProperties { RedirectUri = returnUrl });
            });

            // LOGOUT endpoint
            app.MapGet("/logout", async (HttpContext context, string? returnUrl) =>
            {
                var signInManager = context.RequestServices.GetRequiredService<SignInManager<Person>>();
                await signInManager.SignOutAsync();

                await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                await context.SignOutAsync(IdentityConstants.ExternalScheme);
                await context.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);
                await context.SignOutAsync(IdentityConstants.TwoFactorRememberMeScheme);

                returnUrl ??= "/account/login";
                context.Response.Redirect(returnUrl);
            })
            .RequireAuthorization();


            return app;
        }
    }
}
