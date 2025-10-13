using FcmsPortal.Models;
using FcmsPortalUI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;


namespace FcmsPortalUI.Services
{
    public class AccountInvitationService : IAccountInvitationService
    {
        private readonly FcmsPortalUIContext _context;
        private readonly UserManager<Person> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public AccountInvitationService(
            FcmsPortalUIContext context,
            UserManager<Person> userManager,
            RoleManager<IdentityRole<int>> roleManager,
               IEmailSender emailSender,
            IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _config = config;
        }

        public async Task<AccountInvitation?> CreateInvitationAsync(
            int personId, string email, string role, int sentById)
        {
            //Ensure user exists in Identity (Person = ApplicationUser)
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = await _context.Persons.FindAsync(personId);
                if (user == null)
                    throw new InvalidOperationException("Person not found.");

                user.UserName = email;
                user.Email = email;
                user.EmailConfirmed = false;

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    throw new InvalidOperationException(
                        $"Failed to create user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole<int>(role));


            if (!await _userManager.IsInRoleAsync(user, role))
                await _userManager.AddToRoleAsync(user, role);

            //Generate a secure confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

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

            await SendInvitationEmailAsync(invitation);

            return invitation;
        }

        private async Task SendInvitationEmailAsync(AccountInvitation invitation)
        {
            var baseUrl = _config["AppSettings:BaseUrl"] ?? "https://localhost:5001";
            var link = $"{baseUrl}/register?token={Uri.EscapeDataString(invitation.Token)}&email={Uri.EscapeDataString(invitation.Email)}";

            var subject = "Your School Portal Account Invitation";
            var body = $@"
                <html>
                <body>
                    <h2>Welcome!</h2>
                    <p>You have been invited to join the school portal as a <strong>{invitation.Role}</strong>.</p>
                    <p>Please click the button below to complete your registration:</p>
                    <p>
                        <a href='{link}' style='background-color:#1b6ec2;color:white;padding:10px 20px;
                        text-decoration:none;border-radius:5px;'>Complete Registration</a>
                    </p>
                    <p>This link will expire on <b>{invitation.ExpiryDate:MMMM dd, yyyy}</b>.</p>
                </body>
                </html>";

            await _emailSender.SendEmailAsync(invitation.Email, subject, body);
        }

        public async Task<AccountInvitation?> GetByTokenAsync(string token)
        {
            return await _context.AccountInvitations
                .FirstOrDefaultAsync(i => i.Token == token && !i.IsUsed && i.ExpiryDate > DateTime.UtcNow);
        }

        public async Task<bool> MarkAsUsedAsync(string token)
        {
            var invite = await _context.AccountInvitations
                .FirstOrDefaultAsync(i => i.Token == token);

            if (invite == null)
                return false;

            invite.IsUsed = true;
            await _context.SaveChangesAsync();
            return true;
        }

        //Validate token only
        public async Task<bool> IsValidTokenAsync(string token)
        {
            var invite = await GetByTokenAsync(token);
            return invite != null;
        }
    }
}
