using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OfferManagement.Application.Authentication.AuthServices.AdminAuth;
using OfferManagement.Application.Authentication.AuthServices.CompanyAuth;
using OfferManagement.Application.Authentication.AuthServices.UserAuth;
using OfferManagement.Application.Authentication.TokenServices;
using OfferManagement.Application.AuthServices.JWT.Models;
using OfferManagement.Common.Extensions;

namespace OfferManagement.Common.Middlewares
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public TokenMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("AuthToken", out var accessToken))
            {
                context.Request.Headers["Authorization"] = "Bearer " + accessToken;
            }

            var originalBodyStream = context.Response.Body;
            using var responseBuffer = new MemoryStream();
            context.Response.Body = responseBuffer;

            await _next(context);

            if (context.Response.StatusCode == 401 &&
                context.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken) &&
                context.Request.Cookies.TryGetValue("AuthToken", out var expiredAccessToken))
            {
                var tokens = new AuthTokens
                {
                    AccessToken = expiredAccessToken,
                    RefreshToken = refreshToken
                };

                AuthTokens? newTokens = null;

                using (var scope = _scopeFactory.CreateScope())
                {
                    var userAuthService = scope.ServiceProvider.GetRequiredService<IUserAuthService>();
                    var companyAuthService = scope.ServiceProvider.GetRequiredService<ICompanyAuthService>();
                    var adminAuthService = scope.ServiceProvider.GetRequiredService<IAdminAuthService>();
                    var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

                    var role = tokenService.GetPrincipalFromExpiredToken(expiredAccessToken).GetRoleFromPrincipal();

                    if (role == "User")
                    {
                        newTokens = await userAuthService.RotateTokensAsync(tokens, context.RequestAborted);
                    }
                    else if (role == "Company")
                    {
                        newTokens = await companyAuthService.RotateTokensAsync(tokens, context.RequestAborted);
                    }
                    else if (role == "Admin")
                    {
                        newTokens = await adminAuthService.RotateTokensAsync(tokens, context.RequestAborted);
                    }
                }

                if (newTokens != null)
                {
                    StoreTokens(context, newTokens.AccessToken, newTokens.RefreshToken);

                    context.Response.Body = originalBodyStream;
                    context.Request.Headers["Authorization"] = "Bearer " + newTokens.AccessToken;

                    await _next(context);
                    return;
                }
            }

            responseBuffer.Position = 0;
            await responseBuffer.CopyToAsync(originalBodyStream);
        }

        private void StoreTokens(HttpContext context, string accessToken, string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            context.Response.Cookies.Append("AuthToken", accessToken, cookieOptions);
            context.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
        }
    }




}
