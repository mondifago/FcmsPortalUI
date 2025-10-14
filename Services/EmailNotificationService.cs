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

        public async Task SendAccountInvitationAsync(string to, string token, string role, DateTime expiryDate)
        {
            var baseUrl = _config["AppSettings:BaseUrl"] ?? "http://localhost:5100";

            var registrationUrl = $"{baseUrl}/Account/Register?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(to)}";

            var subject = "Activate Your School Portal Account";
            var body = new StringBuilder();
            body.AppendLine("<h3>Welcome to the School Portal!</h3>");
            body.AppendLine($"<p>You have been invited to create an account as a <strong>{role}</strong>.</p>");
            body.AppendLine($"<p><a href='{registrationUrl}' style='background:#1b6ec2;color:white;padding:10px 20px;text-decoration:none;border-radius:5px;'>Complete Registration</a></p>");
            body.AppendLine($"<p>This link will expire on <strong>{expiryDate:MMMM dd, yyyy}</strong>.</p>");

            await _emailService.SendEmailAsync(to, subject, body.ToString());
        }


        public async Task SendSchoolFeesReminderAsync(string to, string studentName, decimal amount, DateTime dueDate)
        {
            var subject = "School Fees Payment Reminder";
            var body = $@"
                <p>Dear {studentName},</p>
                <p>This is a friendly reminder that your school fees of <b>{amount:C}</b> are due on {dueDate:MMMM dd, yyyy}.</p>
                <p>Please make payment before the due date to avoid penalties.</p>";
            await _emailService.SendEmailAsync(to, subject, body);
        }

        public async Task SendEventNotificationAsync(string to, string eventTitle, string details)
        {
            var subject = $"Upcoming Event: {eventTitle}";
            var body = $"<p>Dear Guardian/Student,</p><p>{details}</p>";
            await _emailService.SendEmailAsync(to, subject, body);
        }

        public async Task SendResultNotificationAsync(string to, string studentName, string reportUrl)
        {
            var subject = "Your End-of-Semester Results Are Ready";
            var body = $@"
                <p>Dear {studentName},</p>
                <p>Your academic report card is now available.</p>
                <p><a href='{reportUrl}'>View Results</a></p>";
            await _emailService.SendEmailAsync(to, subject, body);
        }

        public async Task SendNewsletterAsync(string to, string subject, string htmlContent)
        {
            await _emailService.SendEmailAsync(to, subject, htmlContent);
        }
    }
}