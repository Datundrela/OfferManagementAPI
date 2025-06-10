using OfferManagement.Domain.Entities;
using System.Security.Claims;

namespace OfferManagement.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetIdFromPrincipal(this ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException("ID not found in token.");

            return int.Parse(idClaim.Value);
        }

        public static string GetEmailFromPrincipal(this ClaimsPrincipal user)
        {
            var emailClaim = user.FindFirst(ClaimTypes.Email);
            if (emailClaim == null)
                throw new UnauthorizedAccessException("Email not found in token.");

            return emailClaim.Value;
        }

        public static string GetRoleFromPrincipal(this ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            if (roleClaim == null)
                throw new UnauthorizedAccessException("Role not found in token.");

            return roleClaim.Value;
        }
    }

}
