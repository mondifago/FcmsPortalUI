using FcmsPortal.Models;
using FcmsPortalUI.Services;
using Microsoft.AspNetCore.Identity;

namespace FcmsPortalUI.Components.Account
{
    public sealed class IdentityEmailSender : IEmailSender<Person>
    {
        private readonly IEmailService _emailService;

        public IdentityEmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public Task SendConfirmationLinkAsync(Person user, string email, string confirmationLink) =>
            _emailService.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

        public Task SendPasswordResetLinkAsync(Person user, string email, string resetLink) =>
            _emailService.SendEmailAsync(
                email,
                "Reset your password",
                $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

        public Task SendPasswordResetCodeAsync(Person user, string email, string resetCode) =>
            _emailService.SendEmailAsync(
                email,
                "Reset your password",
                $"Please reset your password using the following code: {resetCode}");
    }
}
