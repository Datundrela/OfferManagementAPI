using FluentValidation;
using OfferManagement.Application.EntityServices.Companies.Models;

namespace OfferManagement.Common.Validations
{
    internal class CompanyPutRequestValidator : AbstractValidator<CompanyPutRequestModel>
    {
        public CompanyPutRequestValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("Id is required")
                .GreaterThan(0).WithMessage("Id must be greater than 0");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Company Name is Required")
                .MaximumLength(100).WithMessage("Name should not be longer than 100 charachters");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required")
                .MaximumLength(100).WithMessage("Email should not be longer than 100 characters");

        }
    }
}
