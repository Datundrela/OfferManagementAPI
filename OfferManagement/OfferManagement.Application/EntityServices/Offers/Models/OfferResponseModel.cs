using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Offers.Models
{
    public class OfferResponseModel
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
