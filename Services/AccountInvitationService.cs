using FcmsPortal.Models;
using FcmsPortalUI.Data;
using Microsoft.EntityFrameworkCore;

namespace FcmsPortalUI.Services
{
    public class AccountInvitationService : IAccountInvitationService
    {
        private readonly IDbContextFactory<FcmsPortalUIContext> _contextFactory;
        private readonly IEmailNotificationService _emailService;
        private readonly IConfiguration _configuration;

        public AccountInvitationService(
            IDbContextFactory<FcmsPortalUIContext> contextFactory,
            IEmailNotificationService emailService,
            IConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<AccountInvitation?> CreateInvitationAsync(
            int personId, string email, string role, int sentById)
        {
            var token = Guid.NewGuid().ToString();

            var invitation = new AccountInvitation
            {
                PersonId = personId,
                Token = token,
                Email = email,
                Role = role,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsUsed = false,
                SentByAccountId = sentById
            };

            await using (var context = await _contextFactory.CreateDbContextAsync())
            {
                context.AccountInvitations.Add(invitation);
                await context.SaveChangesAsync();
            }

            await SendInvitationAsync(invitation);

            return invitation;
        }

        public async Task SendInvitationAsync(AccountInvitation invitation)
        {
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var registrationUrl = $"{baseUrl}/register?token={Uri.EscapeDataString(invitation.Token)}";

            // Get person name from database (you'll need to add this logic based on Role)
            var personName = await GetPersonNameAsync(invitation.PersonId);

            await _emailService.SendInvitationEmailAsync(
                invitation.Email,
                personName,
                invitation.Role,
                registrationUrl);
        }

        public async Task<AccountInvitation?> GetByTokenAsync(string token)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.AccountInvitations
                .FirstOrDefaultAsync(i => i.Token == token && !i.IsUsed && i.ExpiryDate > DateTime.UtcNow);
        }

        public async Task<bool> MarkAsUsedAsync(string token)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var invitation = await context.AccountInvitations
                .FirstOrDefaultAsync(i => i.Token == token);

            if (invitation == null)
                return false;

            invitation.IsUsed = true;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsValidTokenAsync(string token)
        {
            var invitation = await GetByTokenAsync(token);
            return invitation != null;
        }

        private async Task<string> GetPersonNameAsync(int personId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var person = await context.Persons.FindAsync(personId);
            return person?.FirstName ?? "User";
        }

        public async Task<List<AccountInvitation>> GetInvitationsForPersonAsync(int personId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.AccountInvitations
                .Where(i => i.PersonId == personId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }
    }
}