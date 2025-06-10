using OfferManagement.Domain;
using OfferManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Offers.Models
{
    public class OfferDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }

        public DateTime UploadDate { get; set; }
        public OfferStatus OfferStatus { get; set; }
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public int? ImageId { get; set; } = null;
        public Image? Image { get; set; } = null;
    }
}
