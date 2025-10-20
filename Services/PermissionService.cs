using FcmsPortal.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FcmsPortalUI.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly AuthenticationStateProvider _auth;
        private readonly UserManager<Person> _userManager;

        public PermissionService(AuthenticationStateProvider auth, UserManager<Person> userManager)
        {
            _auth = auth;
            _userManager = userManager;
        }


        public async Task<ClaimsPrincipal> GetCurrentPrincipalAsync()
        {
            var authState = await _auth.GetAuthenticationStateAsync();
            return authState.User;
        }

        public async Task<int?> GetCurrentUserIdAsync()
        {
            var user = await GetCurrentPrincipalAsync();
            if (user?.Identity?.IsAuthenticated != true) return null;

            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim != null && int.TryParse(idClaim.Value, out var id))
                return id;

            return null;
        }

        public async Task<Person?> GetCurrentPersonAsync()
        {
            var id = await GetCurrentUserIdAsync();
            if (id is null) return null;

            return await _userManager.FindByIdAsync(id.Value.ToString());
        }
    }
}
