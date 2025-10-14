namespace FcmsPortalUI.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody, string? from = null);
    }
}
