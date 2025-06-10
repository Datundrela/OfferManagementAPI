using Microsoft.AspNetCore.Http;
using OfferManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Images
{
    public interface IImageService
    {
        Task<Image> UploadAsync(IFormFile file, CancellationToken cancellationToken);
        Task<Image> GetImageAsync(int imageId, CancellationToken cancellationToken);
        Task DeleteImageAsync(int imageId, CancellationToken cancellationToken);
    }

}
