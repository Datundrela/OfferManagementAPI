using OfferManagement.Application.Authentication.AuthServices.Models;
using OfferManagement.Application.AuthServices.JWT.Models;

namespace OfferManagement.Application.Authentication.AuthServices.UserAuth
{
    public interface IUserAuthService
    {
        Task<AuthTokens> RegisterAsync(UserRegisterRequestModel request, CancellationToken cancellationToken);
        Task<LoginResponseModel> LoginAsync(LoginRequestModel request, CancellationToken cancellationToken);
        Task<AuthTokens> RotateTokensAsync(AuthTokens tokens, CancellationToken cancellationToken);
    }
}
