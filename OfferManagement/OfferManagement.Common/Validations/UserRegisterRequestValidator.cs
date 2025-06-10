using FluentValidation;
using OfferManagement.Application.Authentication.AuthServices.Models;

namespace OfferManagement.Common.Validations
{
    public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequestModel>
    {
        public UserRegisterRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name must not contain more than 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name must not contain more than 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .MaximumLength(100).WithMessage("Email must not contain more than 50 characters")
                .EmailAddress().WithMessage("Invalid email formaat");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
