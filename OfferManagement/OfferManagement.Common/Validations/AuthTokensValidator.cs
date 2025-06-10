using FluentValidation;
using OfferManagement.Application.AuthServices.JWT.Models;

namespace OfferManagement.Common.Validations
{
    public class AuthTokensValidator : AbstractValidator<AuthTokens>
    {
        public AuthTokensValidator()
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage("Access token is required");

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required");
        }
    }
}
