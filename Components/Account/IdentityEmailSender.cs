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
        "Confirm Your Email - Future Champions Model School",
        BuildEmailTemplate(
            $"Hello {user.FirstName}",
            "Please confirm your email address to complete your email change.",
            "Confirm Email",
            confirmationLink,
            "If you did not create an account, please ignore this email."
        ));

        public Task SendPasswordResetLinkAsync(Person user, string email, string resetLink) =>
            _emailService.SendEmailAsync(
                email,
                "Reset Your Password - Future Champions Model School",
                BuildEmailTemplate(
                    $"Hello {user.FirstName}",
                    "We received a request to reset your password. Click the button below to create a new password.",
                    "Reset Password",
                    resetLink,
                    "If you did not request a password reset, please ignore this email. Your password will remain unchanged."
                ));

        public Task SendPasswordResetCodeAsync(Person user, string email, string resetCode) =>
            _emailService.SendEmailAsync(
                email,
                "Your Password Reset Code - Future Champions Model School",
                BuildEmailTemplate(
                    $"Hello {user.FirstName}",
                    $"Your password reset code is: <strong style='font-size: 24px; letter-spacing: 3px;'>{resetCode}</strong>",
                    null,
                    null,
                    "If you did not request this code, please ignore this email."
                ));

        private string BuildEmailTemplate(string greeting, string message, string? buttonText, string? buttonUrl, string footerNote)
        {
            var buttonHtml = !string.IsNullOrEmpty(buttonText) && !string.IsNullOrEmpty(buttonUrl)
                ? $@"
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{buttonUrl}' style='display: inline-block; background-color: #28a745; color: #ffffff; padding: 15px 40px; text-decoration: none; border-radius: 25px; font-weight: bold; font-size: 16px;'>{buttonText}</a>
            </div>
            "
                    : "";
            return $@"
    <!DOCTYPE html>
    <html>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        </head>
        <body style='font-family: Poppins, Arial, sans-serif; background-color: #f5f7fb; margin: 0; padding: 0;'>
            <div style='max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); overflow: hidden;'>
                <div style='padding: 40px 30px; color: #333333;'>
                    <h2 style='margin: 0 0 50px 0; font-size: 28px; color: #ff0000; text-align: center;'>Future Champions Model School</h2>
                    <h3 style='color: #28a745; margin-top: 0;'>{greeting}</h3>
                    <p style='line-height: 1.6; margin-bottom: 15px;'>{message}</p>
                    {buttonHtml}
                    <div style='margin-top: 30px; font-size: 14px; color: #6c757d; padding: 15px; background-color: #fff3cd; border-left: 4px solid #ffc107; border-radius: 4px;'>
                        <strong>Note:</strong> {footerNote}
                    </div>
                </div>
                <div style='background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 12px; color: #6c757d; border-top: 1px solid #dee2e6;'>
                    <p style='margin: 5px 0;'>This is an automated message from FCMS Portal.</p>
                    <p style='margin: 5px 0;'>Please do not reply to this email.</p>
                </div>
            </div>
        </body>
    </html>";
        }
    }
}
