using FluentValidation;
using OfferManagement.Application.EntityServices.Companies.Models;

namespace OfferManagement.Common.Validations
{
    public class CreateCompanyDTOValidator : AbstractValidator<CreateCompanyDTO>
    {
        public CreateCompanyDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Company Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 500 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is requierd");
        }
    }
}
