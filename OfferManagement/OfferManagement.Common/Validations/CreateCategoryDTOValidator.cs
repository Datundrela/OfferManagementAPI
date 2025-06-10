using FluentValidation;
using OfferManagement.Application.EntityServices.Categories.Models;
using OfferManagement.Application.EntityServices.Offers.Models;

namespace OfferManagement.Common.Validations
{
    public class CreateCategoryDTOValidator : AbstractValidator<CreateCategoryDTO>
    {
        public CreateCategoryDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category Name is required")
                .MaximumLength(1100).WithMessage("Name cannot exceed 100 characters");
        }
    }
}
