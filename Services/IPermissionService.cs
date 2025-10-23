using FcmsPortal.Models;
using System.Security.Claims;

namespace FcmsPortalUI.Services
{
    public interface IPermissionService
    {
        Task<ClaimsPrincipal> GetCurrentPrincipalAsync();
        Task<int?> GetCurrentUserIdAsync();
        Task<Person?> GetCurrentPersonAsync();
        Task<bool> IsInRoleAsync(string roleName);
        Task<bool> IsInAnyRoleAsync(params string[] roleNames);
    }
}
