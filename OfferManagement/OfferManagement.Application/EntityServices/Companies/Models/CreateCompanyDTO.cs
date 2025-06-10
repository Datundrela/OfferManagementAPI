using OfferManagement.Domain.Entities;
using OfferManagement.Domain;

namespace OfferManagement.Application.EntityServices.Companies.Models
{
    public class CreateCompanyDTO
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
