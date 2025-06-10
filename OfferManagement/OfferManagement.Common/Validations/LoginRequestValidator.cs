using FluentValidation;
using OfferManagement.Application.Authentication.AuthServices.Models;

namespace OfferManagement.Common.Validations
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestModel>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
