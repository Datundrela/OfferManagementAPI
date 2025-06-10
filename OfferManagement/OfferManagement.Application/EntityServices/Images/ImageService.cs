using Microsoft.AspNetCore.Http;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using System;

namespace OfferManagement.Application.EntityServices.Images
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Image> UploadAsync(IFormFile file, CancellationToken cancellationToken)
        {
            var appRoot = Directory.GetCurrentDirectory();

            var uploadsFolder = Path.Combine(appRoot, "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var image = new Image
            {
                FileName = file.FileName,
                FilePath = "/uploads/" + uniqueFileName
            };

            await _unitOfWork.ImageRepository.AddAsync(image, cancellationToken);
            return image;
        }

        public async Task<Image> GetImageAsync(int imageId, CancellationToken cancellationToken)
        {
            Image? image = await _unitOfWork.ImageRepository.GetByIdAsync(imageId, cancellationToken);

            return image;
        }

        public async Task DeleteImageAsync(int imageId, CancellationToken cancellationToken)
        {
            Image? image = await _unitOfWork.ImageRepository.GetByIdAsync(imageId, cancellationToken);

            var appRoot = Directory.GetCurrentDirectory();

            var imagePath = Path.Combine(appRoot, "wwwroot", image.FilePath.TrimStart('/'));

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }

            _unitOfWork.ImageRepository.Delete(image);
        }
    }

}
