using FluentValidation;
using OfferManagement.Application.EntityServices.Users.Models;

namespace OfferManagement.Common.Validations
{
    public class TransactionRequestValidator : AbstractValidator<TransactionRequestModel>
    {
        public TransactionRequestValidator()
        {
            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount is required")
                .GreaterThan(0).WithMessage("Amount should be greater than zero");
        }
    }
}
