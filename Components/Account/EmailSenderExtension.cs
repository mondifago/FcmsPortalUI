using FcmsPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace FcmsPortalUI.Components.Account
{
    public class EmailSenderExtension : IEmailSender
    {
        private readonly IEmailSender<Person> _inner;

        public EmailSenderExtension(IEmailSender<Person> inner)
        {
            _inner = inner;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var dummy = new Person { Email = email, UserName = email };
            // Use the confirmation link API to send a custom message
            await _inner.SendConfirmationLinkAsync(dummy, email, htmlMessage);
        }
    }
}