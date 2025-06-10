using OfferManagement.Domain.Entities;
using OfferManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OfferManagement.Application.EntityServices.Offers.Models
{
    public class AddOfferRequestModel
    {
        public string Title { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public IFormFile? Image { get; set; } = null;

        public DateTime ExpiryDate { get; set; }

        public int CategoryId { get; set; }
    }
}
