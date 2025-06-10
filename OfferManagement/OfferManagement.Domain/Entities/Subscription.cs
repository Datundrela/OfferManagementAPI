using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Domain.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
