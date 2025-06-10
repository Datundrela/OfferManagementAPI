using FluentValidation;
using OfferManagement.Application.EntityServices.Users.Models;

namespace OfferManagement.Common.Validations
{
    public class UserPutRequestValidator : AbstractValidator<UserPutRequestModel>
    {
        public UserPutRequestValidator()
        {
            RuleFor(u => u.Id)
                .NotEmpty().WithMessage("Id is required")
                .GreaterThanOrEqualTo(1).WithMessage("Id should be greater than zero");

            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("First Name is required")
                .MaximumLength(50).WithMessage("Fisrt Name should not be longer than 50 characters");

            RuleFor(u => u.LastName)
                .NotEmpty().WithMessage("Last Name is required")
                .MaximumLength(50).WithMessage("Last Name should not be longer than 50 characters");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required")
                .MaximumLength(100).WithMessage("Email should not be longer than 100 characters");
        }
    }
}
