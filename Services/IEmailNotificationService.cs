namespace FcmsPortalUI.Services
{
    public interface IEmailNotificationService
    {
        Task SendAccountInvitationAsync(string to, string token, string role, DateTime expiryDate);
        Task SendSchoolFeesReminderAsync(string to, string studentName, decimal amount, DateTime dueDate);
        Task SendEventNotificationAsync(string to, string eventTitle, string details);
        Task SendResultNotificationAsync(string to, string studentName, string reportUrl);
        Task SendNewsletterAsync(string to, string subject, string htmlContent);
    }
}
