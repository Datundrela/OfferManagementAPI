using FluentValidation;
using OfferManagement.Application.EntityServices.Purchases.Models;

namespace OfferManagement.Common.Validations
{
    public class PurchaseOfferRequestValidator : AbstractValidator<PurchaseOfferRequestModel>
    {
        public PurchaseOfferRequestValidator()
        {
            RuleFor(x => x.Quantity)
                .NotEmpty()
                .GreaterThan(0).WithMessage("Quantity must be greater than zero");
        }
    }
}
