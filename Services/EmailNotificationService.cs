using FcmsPortal.Models;
using System.Text;

namespace FcmsPortalUI.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public EmailNotificationService(IEmailService emailService, IConfiguration config)
        {
            _emailService = emailService;
            _config = config;
        }

        public async Task SendAccountInvitationAsync(Person user, string to, string token, string role, DateTime expiryDate)
        {
            var baseUrl = _config["AppSettings:BaseUrl"] ?? "http://localhost:5100";
            var registrationUrl = $"{baseUrl}/Account/Register?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(to)}";

            var subject = "Activate Your School Portal Account";

            var body = new StringBuilder();
            body.AppendLine("<h3>Welcome to Future Champions Model School Portal!</h3>");
            body.AppendLine($"<p>You have been invited to create an account as a <strong>{role}</strong>.</p>");
            body.AppendLine($"<p>This link will expire on <strong>{expiryDate:MMMM dd, yyyy}</strong>.</p>");

            var html = BuildEmailTemplate(
                greeting: $"Hello {user.FirstName}",
                message: body.ToString(),
                buttonText: "Complete Registration",
                buttonUrl: registrationUrl,
                footerNote: "If you did not expect this invitation, you may ignore this email."
            );

            await _emailService.SendEmailAsync(to, subject, html);
        }

        public async Task SendSchoolFeesReminderAsync(Person user, string to, decimal amount, DateTime dueDate)
        {
            var subject = "School Fees Payment Reminder";

            var body = $@"
                <p>This is a friendly reminder that your school fees of <b>{amount:C}</b> are due on {dueDate:MMMM dd, yyyy}.</p>
                <p>Please make payment before the due date to avoid penalties.</p>";

            var html = BuildEmailTemplate(
                greeting: $"Hello {user.FirstName}",
                message: body,
                buttonText: null,
                buttonUrl: null,
                footerNote: "If you have already made payment, please ignore this reminder."
            );

            await _emailService.SendEmailAsync(to, subject, html);
        }

        public async Task SendEventNotificationAsync(Person user, string to, string eventTitle, string details)
        {
            var subject = $"Upcoming Event: {eventTitle}";

            var body = $"<p>{details}</p>";

            var html = BuildEmailTemplate(
                greeting: $"Hello {user.FirstName}",
                message: body,
                buttonText: null,
                buttonUrl: null,
                footerNote: "This is an automated notification from the school portal."
            );

            await _emailService.SendEmailAsync(to, subject, html);
        }

        public async Task SendResultNotificationAsync(Person user, string to, string reportUrl)
        {
            var subject = "Your End-of-Semester Results Are Ready";

            var body = $@"
                <p>Your academic report card is now available.</p>
                <p><a href='{reportUrl}'>View Results</a></p>";

            var html = BuildEmailTemplate(
                greeting: $"Hello {user.FirstName}",
                message: body,
                buttonText: "View Results",
                buttonUrl: reportUrl,
                footerNote: "If you have trouble accessing the report, please contact the school admin."
            );

            await _emailService.SendEmailAsync(to, subject, html);
        }

        public async Task SendNewsletterAsync(string to, string subject, string htmlContent)
        {
            await _emailService.SendEmailAsync(to, subject, htmlContent);
        }

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
                            <div style='line-height: 1.6; margin-bottom: 15px;'>{message}</div>
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
