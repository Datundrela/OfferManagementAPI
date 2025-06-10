using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Domain.Entities
{
        public class Purchase
        {
            public int Id { get; set; }

            public int Quantity { get; set; }

            public decimal TotalPrice { get; set; }

            public DateTime PurchaseDate { get; set; }

            public bool IsRefunded { get; set; } = false;


            public int UserId { get; set; }

            public User User { get; set; }

            public int OfferId { get; set; }

            public Offer Offer { get; set; }
        }
}
