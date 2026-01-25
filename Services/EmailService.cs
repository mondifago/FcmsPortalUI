using System.Diagnostics;
using System.Net;
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
            try
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

                // Development-only file pickup (optional)
                if (_env.IsDevelopment() && saveToFile)
                {
                    var outputDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "emails");
                    Directory.CreateDirectory(outputDir);

                    using var fileClient = new SmtpClient
                    {
                        DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                        PickupDirectoryLocation = outputDir
                    };

                    await fileClient.SendMailAsync(message);
                    return;
                }

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true, // REQUIRED for 587 (STARTTLS)
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000
                };

                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EMAIL SEND FAILED");
                Console.WriteLine(ex.ToString());
                Debug.WriteLine(ex.ToString());
                throw; // bubble up to UI/logs
            }
        }
    }
}
