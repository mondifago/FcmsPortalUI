using FcmsPortal.Models;

namespace FcmsPortalUI.Services
{
    public interface IAccountInvitationService
    {
        Task<AccountInvitation?> CreateInvitationAsync(int personId, string email, string role, int sentById);
        Task<AccountInvitation?> GetByTokenAsync(string token);
        Task<bool> MarkAsUsedAsync(string token);
        Task<bool> IsValidTokenAsync(string token);
    }
}
