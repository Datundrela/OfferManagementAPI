using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Images.Models
{
    public class ImageUploadDto
    {
        public IFormFile File { get; set; } = default!;
        public int? CompanyId { get; set; }
        public int? OfferId { get; set; }
    }

}
