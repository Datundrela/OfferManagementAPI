using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Subscriptions.Models
{
    public class CreateSubscriptionDTO
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
    }
}
