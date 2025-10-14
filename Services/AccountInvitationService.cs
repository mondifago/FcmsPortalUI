using FcmsPortal.Models;
using FcmsPortalUI.Data;
using Microsoft.EntityFrameworkCore;

namespace FcmsPortalUI.Services
{
    public class AccountInvitationService : IAccountInvitationService
    {
        private readonly FcmsPortalUIContext _context;
        private readonly IEmailNotificationService _emailService;
        private readonly IConfiguration _configuration;

        public AccountInvitationService(
            FcmsPortalUIContext context,
            IEmailNotificationService emailService,
            IConfiguration configuration)
        {
            _context = context;
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

            _context.AccountInvitations.Add(invitation);
            await _context.SaveChangesAsync();

            await SendInvitationAsync(invitation);

            return invitation;
        }

        public async Task SendInvitationAsync(AccountInvitation invitation)
        {
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var registrationUrl = $"{baseUrl}/register?token={Uri.EscapeDataString(invitation.Token)}";

            // Get person name from database (you'll need to add this logic based on Role)
            var personName = await GetPersonNameAsync(invitation.PersonId, invitation.Role);

            await _emailService.SendInvitationEmailAsync(
                invitation.Email,
                personName,
                invitation.Role,
                registrationUrl);
        }

        public async Task<AccountInvitation?> GetByTokenAsync(string token)
        {
            return await _context.AccountInvitations
                .FirstOrDefaultAsync(i => i.Token == token && !i.IsUsed && i.ExpiryDate > DateTime.UtcNow);
        }

        public async Task<bool> MarkAsUsedAsync(string token)
        {
            var invitation = await _context.AccountInvitations
                .FirstOrDefaultAsync(i => i.Token == token);

            if (invitation == null)
                return false;

            invitation.IsUsed = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsValidTokenAsync(string token)
        {
            var invitation = await GetByTokenAsync(token);
            return invitation != null;
        }

        private async Task<string> GetPersonNameAsync(int personId, string role)
        {
            // Logic to get person name based on role
            if (role == "Teacher" || role == "Admin")
            {
                var staff = await _context.Staff.FindAsync(personId);
                return staff?.Person.FirstName ?? "User";
            }
            if (role == "Student")
            {
                var student = await _context.Students.FindAsync(personId);
                return student?.Person.FirstName ?? "User";
            }
            if (role == "Guardian")
            {
                var guardian = await _context.Guardians.FindAsync(personId);
                return guardian?.Person.FirstName ?? "User";
            }
            return "User";
        }

        public async Task<List<AccountInvitation>> GetInvitationsForPersonAsync(int personId)
        {
            return await _context.AccountInvitations
                .Where(i => i.PersonId == personId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }
    }
}