using FluentValidation;
using OfferManagement.Application.EntityServices.Offers.Models;

namespace OfferManagement.Common.Validations
{
    public class AddOfferRequestValidator : AbstractValidator<AddOfferRequestModel>
    {
        public AddOfferRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(150).WithMessage("Title cannot exceed 150 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Price)
                .NotEmpty()
                .GreaterThan(0).WithMessage("Price must be greater than zero");

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .GreaterThan(0).WithMessage("Quantity must be greater than zero");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future");
        }
    }
}
