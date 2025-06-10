using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Moq;
using OfferManagement.Application.EntityServices.Images;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using System.Text;

namespace OfferManagement.Application.Tests.ImageTests
{
    public class ImageServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IImageRepository> _imageRepoMock;
        private readonly ImageService _imageService;

        public ImageServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _imageRepoMock = new Mock<IImageRepository>();

            _unitOfWorkMock.Setup(u => u.ImageRepository).Returns(_imageRepoMock.Object);

            _imageService = new ImageService(_unitOfWorkMock.Object);
        }

        private IFormFile CreateFakeFormFile(string fileName = "test.jpg", string content = "fake image content")
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "file", fileName);
        }

        [Fact]
        public async Task UploadAsync_ShouldStoreFile_AndSaveToDb()
        {
            // Arrange
            var file = CreateFakeFormFile();
            Image savedImage = null!;

            _imageRepoMock.Setup(r => r.AddAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()))
                .Callback<Image, CancellationToken>((img, _) => savedImage = img)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _imageService.UploadAsync(file, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.FileName.Should().Be(file.FileName);
            result.FilePath.Should().Contain("/uploads/");
            savedImage.Should().BeEquivalentTo(result);
            File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", result.FilePath.TrimStart('/'))).Should().BeTrue();
        }

        [Fact]
        public async Task GetImageAsync_ShouldReturnImage_WhenExists()
        {
            var image = new Image { Id = 1, FileName = "test.jpg", FilePath = "/uploads/test.jpg" };

            _imageRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(image);

            var result = await _imageService.GetImageAsync(1, CancellationToken.None);

            result.Should().BeEquivalentTo(image);
        }

        [Fact]
        public async Task GetImageAsync_ShouldReturnNull_WhenNotExists()
        {
            _imageRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Image)null!);

            var result = await _imageService.GetImageAsync(123, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteImageAsync_ShouldDeleteFile_AndFromDb()
        {
            // Arrange
            var fileName = Guid.NewGuid() + ".jpg";
            var filePath = Path.Combine("wwwroot", "uploads", fileName);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            await File.WriteAllTextAsync(fullPath, "dummy");

            var image = new Image { Id = 2, FileName = "dummy.jpg", FilePath = "/uploads/" + fileName };

            _imageRepoMock.Setup(r => r.GetByIdAsync(image.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(image);

            // Act
            await _imageService.DeleteImageAsync(image.Id, CancellationToken.None);

            // Assert
            File.Exists(fullPath).Should().BeFalse();
            _imageRepoMock.Verify(r => r.Delete(image), Times.Once);
        }
    }
}
