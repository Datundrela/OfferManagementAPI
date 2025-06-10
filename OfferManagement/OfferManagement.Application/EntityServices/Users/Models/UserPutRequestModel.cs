using OfferManagement.Domain.Entities;
using OfferManagement.Domain;

namespace OfferManagement.Application.EntityServices.Users.Models
{
    public class UserPutRequestModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}
