using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Purchases.Models
{
    public class PurchaseDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsRefunded { get; set; }
        public int UserId { get; set; }
        public int OfferId { get; set; }
    }

}
