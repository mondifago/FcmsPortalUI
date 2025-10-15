using System.Net.Mail;

namespace FcmsPortalUI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public EmailService(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody, string? from = null)
        {
            var smtpSection = _config.GetSection("SmtpSettings");
            var smtpHost = smtpSection["Host"] ?? "localhost";
            var smtpPort = int.Parse(smtpSection["Port"] ?? "25");
            var smtpUser = smtpSection["Username"];
            var smtpPass = smtpSection["Password"];
            var smtpFrom = from ?? smtpSection["FromEmail"] ?? "noreply@fcms.com";
            var saveToFile = bool.TryParse(smtpSection["SaveToFile"], out var save) && save;

            var message = new MailMessage
            {
                From = new MailAddress(smtpFrom!),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(to);

            if (_env.IsDevelopment() && saveToFile)
            {
                var outputDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "emails");

                if (!Directory.Exists(outputDir))
                    Directory.CreateDirectory(outputDir);

                using var client = new SmtpClient("localhost")
                {
                    DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                    PickupDirectoryLocation = outputDir
                };

                await client.SendMailAsync(message);
                return;
            }

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = false,
                UseDefaultCredentials = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            await smtpClient.SendMailAsync(message);
        }
    }
}
