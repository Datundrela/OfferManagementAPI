using OfferManagement.Application.EntityServices.Categories.Models;
using OfferManagement.Application.EntityServices.Offers.Models;

namespace OfferManagement.Web.Models
{
    public class UserCategoriesViewModel
    {
        public IEnumerable<CategoryDTO> Categories { get; set; }
        public IEnumerable<int> SubscribedCategoryIds { get; set; }
        public decimal Balance { get; set; }
        public IEnumerable<OfferDTO> Offers { get; set; }
    }

}
