using System.Net;
using System.Net.Mail;

namespace FcmsPortalUI.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(
            IConfiguration configuration,
            ILogger<EmailNotificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendInvitationEmailAsync(
            string recipientEmail,
            string recipientName,
            string role,
            string registrationUrl)
        {
            var subject = "Complete Your Account Registration";
            var expiryDate = DateTime.UtcNow.AddDays(7);

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: 'Poppins', Arial, sans-serif;
            background-color: #f5f7fb;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 30px auto;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            overflow: hidden;
        }}
        .email-header {{
            background-color: #1b6ec2;
            color: #ffffff;
            padding: 30px;
            text-align: center;
        }}
        .email-header h1 {{
            margin: 0;
            font-size: 28px;
        }}
        .email-body {{
            padding: 40px 30px;
            color: #333333;
        }}
        .email-body p {{
            line-height: 1.6;
            margin-bottom: 15px;
        }}
        .btn-register {{
            display: inline-block;
            background-color: #28a745;
            color: #ffffff;
            padding: 15px 40px;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
            margin: 20px 0;
            font-size: 16px;
        }}
        .button-container {{
            text-align: center;
            margin: 30px 0;
        }}
        .link-text {{
            word-break: break-all;
            color: #006bb7;
            font-size: 14px;
            background-color: #f8f9fa;
            padding: 10px;
            border-radius: 4px;
            margin: 20px 0;
        }}
        .note {{
            margin-top: 30px;
            font-size: 14px;
            color: #6c757d;
            padding: 15px;
            background-color: #fff3cd;
            border-left: 4px solid #ffc107;
            border-radius: 4px;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            font-size: 12px;
            color: #6c757d;
            border-top: 1px solid #dee2e6;
        }}
        .footer p {{
            margin: 5px 0;
        }}
        .role-badge {{
            background-color: #007bff;
            color: white;
            padding: 5px 15px;
            border-radius: 20px;
            font-weight: bold;
            display: inline-block;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <h1>🎓 Welcome to School Portal!</h1>
        </div>
        
        <div class='email-body'>
            <p>Hello <strong>{recipientName}</strong>,</p>
            
            <p>You have been invited to create an account in our school portal as a <span class='role-badge'>{role}</span>.</p>
            
            <p>To complete your registration and set your password, please click the button below:</p>
            
            <div class='button-container'>
                <a href='{registrationUrl}' class='btn-register'>Complete Registration</a>
            </div>
            
            <p style='margin-top: 30px;'>If the button doesn't work, you can copy and paste this link into your browser:</p>
            <div class='link-text'>{registrationUrl}</div>
            
            <div class='note'>
                <strong>⏰ Important:</strong> This invitation link will expire on <strong>{expiryDate:MMMM dd, yyyy 'at' hh:mm tt}</strong>.
            </div>
        </div>
        
        <div class='footer'>
            <p><strong>Need help?</strong> Contact the school administration.</p>
            <p>If you did not expect this invitation, please ignore this email.</p>
            <p>&copy; {DateTime.Now.Year} School Portal. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(recipientEmail, subject, htmlBody);
        }

        public async Task SendFeeReminderAsync(string email, string studentName, decimal amount, DateTime dueDate)
        {
            var subject = "School Fee Payment Reminder";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: 'Poppins', Arial, sans-serif;
            background-color: #f5f7fb;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 30px auto;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            padding: 40px 30px;
        }}
        h2 {{
            color: #1b6ec2;
        }}
        .amount {{
            font-size: 24px;
            color: #28a745;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <h2>💰 Fee Payment Reminder</h2>
        <p>Dear Parent/Guardian,</p>
        <p>This is a reminder that the school fee payment of <span class='amount'>${amount:N2}</span> for <strong>{studentName}</strong> is due on <strong>{dueDate:MMMM dd, yyyy}</strong>.</p>
        <p>Please make the payment at your earliest convenience.</p>
        <p>Thank you for your cooperation.</p>
    </div>
</body>
</html>";

            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendEventNotificationAsync(string email, string eventTitle, DateTime eventDate)
        {
            var subject = $"Upcoming Event: {eventTitle}";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: 'Poppins', Arial, sans-serif;
            background-color: #f5f7fb;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 30px auto;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            padding: 40px 30px;
        }}
        h2 {{
            color: #1b6ec2;
        }}
        .event-date {{
            background-color: #e3f2fd;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <h2>📅 Event Notification</h2>
        <p>Dear Parent/Guardian,</p>
        <p>This is to inform you about an upcoming school event:</p>
        <h3>{eventTitle}</h3>
        <div class='event-date'>
            <strong>Date:</strong> {eventDate:MMMM dd, yyyy 'at' hh:mm tt}
        </div>
        <p>We look forward to your participation!</p>
    </div>
</body>
</html>";

            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendResultsNotificationAsync(string email, string studentName, string term)
        {
            var subject = $"Academic Results Available - {term}";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: 'Poppins', Arial, sans-serif;
            background-color: #f5f7fb;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 30px auto;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            padding: 40px 30px;
        }}
        h2 {{
            color: #1b6ec2;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <h2>📊 Academic Results</h2>
        <p>Dear Parent/Guardian,</p>
        <p>The academic results for <strong>{studentName}</strong> for <strong>{term}</strong> are now available on the portal.</p>
        <p>Please log in to view the complete results.</p>
    </div>
</body>
</html>";

            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];
            var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");

            using var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                throw;
            }
        }
    }
}