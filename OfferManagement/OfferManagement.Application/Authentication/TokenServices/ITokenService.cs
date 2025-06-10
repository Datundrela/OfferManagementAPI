using OfferManagement.Application.AuthServices.JWT.Models;
using OfferManagement.Domain.Entities;
using System.Security.Claims;

namespace OfferManagement.Application.Authentication.TokenServices
{
    public interface ITokenService
    {
        AuthTokens GenerateTokens(User user);
        AuthTokens GenerateTokens(Company company);
        AuthTokens GenerateTokens(Administrator admin);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        DateTime GetRefreshTokenExpriationTIme();
        DateTime GetAccessTokenExpriationTIme();
    }
}
