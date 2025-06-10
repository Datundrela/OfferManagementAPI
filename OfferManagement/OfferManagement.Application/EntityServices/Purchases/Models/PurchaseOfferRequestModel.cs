using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Purchases.Models
{
    public class PurchaseOfferRequestModel
    {
        public int OfferId { get; set; }
        
        public int UserId { get; set; }

        public int Quantity { get; set; }
    }
}
