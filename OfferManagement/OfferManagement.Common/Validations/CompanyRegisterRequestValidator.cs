using FluentValidation;
using OfferManagement.Application.Authentication.AuthServices.Models;

namespace OfferManagement.Common.Validations
{
    public class CompanyRegisterRequestValidator : AbstractValidator<CompanyRegisterRequestModel>
    {
        public CompanyRegisterRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Company name is required")
                .MaximumLength(100).WithMessage("First name must not contain more than 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .MaximumLength(100).WithMessage("Email must not contain more than 50 characters")
                .EmailAddress().WithMessage("Invalid email formaat");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
