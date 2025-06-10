using OfferManagement.Application.EntityServices.Categories.Models;
using OfferManagement.Application.EntityServices.Companies.Models;
using OfferManagement.Application.Users.Models;

namespace OfferManagement.Web.Models
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<UserDTO> Users { get; set; }
        public IEnumerable<CompanyDTO> Companies { get; set; }
        public IEnumerable<CategoryDTO> Categories { get; set; }
    }

}
