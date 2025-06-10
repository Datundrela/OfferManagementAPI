using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace OfferManagement.Common.Attributes
{
    public class RequireCompanyConfirmationAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }

            var companyConfirmedClaim = user.Claims.FirstOrDefault(c => c.Type.Equals("CompanyConfirmed"))?.Value;

            if (string.IsNullOrEmpty(companyConfirmedClaim) || companyConfirmedClaim != "True")
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
