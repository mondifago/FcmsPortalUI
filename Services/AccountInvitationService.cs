using FcmsPortal.Models;
using FcmsPortalUI.Data;
using Microsoft.EntityFrameworkCore;

namespace FcmsPortalUI.Services
{
    public class AccountInvitationService : IAccountInvitationService
    {
        private readonly IDbContextFactory<FcmsPortalUIContext> _contextFactory;
        private readonly IEmailNotificationService _emailService;

        public AccountInvitationService(
            IDbContextFactory<FcmsPortalUIContext> contextFactory,
            IEmailNotificationService emailService)
        {
            _contextFactory = contextFactory;
            _emailService = emailService;
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
            await _emailService.SendAccountInvitationAsync(
                invitation.Email,
                invitation.Token,
                invitation.Role,
                invitation.ExpiryDate
            );
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
            var invitation = await context.AccountInvitations.FirstOrDefaultAsync(i => i.Token == token);

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