using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Users.Models
{
    public class TransactionRequestModel
    {
        public int UserId { get; set; }

        public decimal Amount { get; set; }
    }
}
