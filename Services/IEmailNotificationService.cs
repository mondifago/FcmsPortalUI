namespace FcmsPortalUI.Services
{
    public interface IEmailNotificationService
    {
        // Account invitation
        Task SendInvitationEmailAsync(string recipientEmail, string recipientName, string role, string registrationUrl);

        // Future school notifications
        Task SendFeeReminderAsync(string email, string studentName, decimal amount, DateTime dueDate);
        Task SendEventNotificationAsync(string email, string eventTitle, DateTime eventDate);
        Task SendResultsNotificationAsync(string email, string studentName, string term);

        // Generic email sender
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
