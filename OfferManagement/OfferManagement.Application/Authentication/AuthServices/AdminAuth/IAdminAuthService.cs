using OfferManagement.Application.Authentication.AuthServices.Models;
using OfferManagement.Application.AuthServices.JWT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Authentication.AuthServices.AdminAuth
{
    public interface IAdminAuthService
    {
        Task<AuthTokens> RegisterAsync(UserRegisterRequestModel request, CancellationToken cancellationToken);
        Task<LoginResponseModel> LoginAsync(LoginRequestModel request, CancellationToken cancellationToken);
        Task<AuthTokens> RotateTokensAsync(AuthTokens tokens, CancellationToken cancellationToken);
    }
}
