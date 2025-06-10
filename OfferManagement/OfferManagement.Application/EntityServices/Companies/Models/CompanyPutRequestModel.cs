using OfferManagement.Domain.Entities;
using OfferManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Companies.Models
{
    public class CompanyPutRequestModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}
