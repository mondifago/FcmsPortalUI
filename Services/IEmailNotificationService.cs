using FcmsPortal.Models;

namespace FcmsPortalUI.Services
{
    public interface IEmailNotificationService
    {
        Task SendAccountInvitationAsync(Person user, string to, string token, string role, DateTime expiryDate);
        Task SendSchoolFeesReminderAsync(Person user, string to, decimal amount, DateTime dueDate);
        Task SendEventNotificationAsync(Person user, string to, string eventTitle, string details);
        Task SendResultNotificationAsync(Person user, string to, string reportUrl);
        Task SendNewsletterAsync(string to, string subject, string htmlContent);
    }
}
