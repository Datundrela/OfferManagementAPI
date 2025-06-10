using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OfferManagement.Application.EntityServices.Images;
using OfferManagement.Application.EntityServices.Offers;
using OfferManagement.Application.EntityServices.Offers.Models;
using OfferManagement.Application.EntityServices.Purchases;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using System.Linq.Expressions;

namespace OfferManagement.Application.Tests.OfferTests
{
    public class OfferByCompanyServiceTests
    {
        private readonly Mock<IOfferService> _offerServiceMock = new();
        private readonly Mock<IPurchaseByUserService> _purchaseServiceMock = new();
        private readonly Mock<IImageService> _imageServiceMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IOfferRepository> _offerRepoMock = new();
        private readonly Mock<IPurchaseRepository> _purchaseRepoMock = new();

        private readonly OfferByCompanyService _sut;

        public OfferByCompanyServiceTests()
        {
            _unitOfWorkMock.Setup(x => x.OfferRepository).Returns(_offerRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.PurchaseRepository).Returns(_purchaseRepoMock.Object);

            _sut = new OfferByCompanyService(
                _offerServiceMock.Object,
                _purchaseServiceMock.Object,
                _imageServiceMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task AddAsync_WithImage_ShouldUploadImageAndAddOffer()
        {
            var request = new AddOfferRequestModel
            {
                Title = "Test Offer",
                Quantity = 10,
                Price = 5.99m,
                Image = Mock.Of<IFormFile>()
            };

            var uploadedImage = new Image { Id = 123, FileName = "img.jpg", FilePath = "/uploads/img.jpg" };

            _imageServiceMock.Setup(x => x.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(uploadedImage);

            var result = await _sut.AddAsync(request, companhyId: 1, CancellationToken.None);

            result.Success.Should().BeTrue();
            result.Message.Should().Be("Offer added successfully.");

            _imageServiceMock.Verify(x => x.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Once);
            _offerRepoMock.Verify(x => x.AddAsync(It.IsAny<Offer>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCompanyAllOffersAsync_ShouldReturnMappedOffers()
        {
            var offers = new List<Offer>
            {
                new Offer { Id = 1, Title = "Apple", Price = 1.2m, CompanyId = 2 },
                new Offer { Id = 2, Title = "Banana", Price = 0.5m , CompanyId = 2}
            };

            _offerRepoMock.Setup(x => x.GetOffersByCompanyIdAsync(2, default)).ReturnsAsync(offers);

            var result = await _sut.GetCompanyAllOffersAsync(2, CancellationToken.None);

            result.Should().HaveCount(2);
            result.Select(r => r.Title).Should().Contain(new[] { "Apple", "Banana" });
        }

        [Fact]
        public async Task CancelOfferAsync_WithPurchases_ShouldRefundAndDelete()
        {
            var offer = new Offer { Id = 1, UploadDate = DateTime.UtcNow, ImageId = 10 };
            var purchases = new List<Purchase> { new Purchase { Id = 1, OfferId = 1 }, new Purchase { Id = 2 , OfferId = 1} };

            _offerRepoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _purchaseRepoMock.Setup(x => x.GetAllByOfferIdAsync(1, CancellationToken.None)).ReturnsAsync(purchases);

            var result = await _sut.CancelOfferAsync(1, CancellationToken.None);

            result.Success.Should().BeTrue();
            result.Message.Should().Be("Offer canceled, and purchases refunded.");

            _purchaseServiceMock.Verify(x => x.CancelPurchaseForCancelledOfferAsync(It.IsAny<Purchase>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelOfferAsync_WithoutPurchases_ShouldDeleteOffer()
        {
            var offer = new Offer { Id = 2, UploadDate = DateTime.UtcNow, ImageId = 5 };

            _offerRepoMock.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
            _purchaseRepoMock.Setup(x => x.GetAllByOfferIdAsync(1, CancellationToken.None)).ReturnsAsync(new List<Purchase>());

            var result = await _sut.CancelOfferAsync(2, CancellationToken.None);

            result.Success.Should().BeTrue();
            result.Message.Should().Be("Offer canceled successfully.");

            _imageServiceMock.Verify(x => x.DeleteImageAsync(5, It.IsAny<CancellationToken>()), Times.Once);
            _offerRepoMock.Verify(x => x.Delete(offer), Times.Once);
        }

        [Fact]
        public async Task CancelOfferAsync_AfterTimeLimit_ShouldReturnFailedResponse()
        {
            var offer = new Offer
            {
                Id = 3,
                UploadDate = DateTime.UtcNow.AddMinutes(-11)
            };

            _offerRepoMock.Setup(x => x.GetByIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(offer);

            var result = await _sut.CancelOfferAsync(3, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.Message.Should().Be("Offer cancellation period has expired.");
        }

    }
}
