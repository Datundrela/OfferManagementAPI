using OfferManagement.Domain;

namespace OfferManagement.Application.Users.Models
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public Role Role { get; set; }
    }
}
