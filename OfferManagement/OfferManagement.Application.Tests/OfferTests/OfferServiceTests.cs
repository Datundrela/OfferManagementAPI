using FluentAssertions;
using Moq;
using OfferManagement.Application.EntityServices.Offers.Models;
using OfferManagement.Application.EntityServices.Offers;
using OfferManagement.Application.Exceptions;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Domain;
using OfferManagement.Infrastructure.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Tests.OfferTests
{
    public class OfferServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IOfferRepository> _offerRepoMock;
        private readonly IOfferService _offerService;

        public OfferServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _offerRepoMock = new Mock<IOfferRepository>();
            _unitOfWorkMock.SetupGet(u => u.OfferRepository).Returns(_offerRepoMock.Object);
            _offerService = new OfferService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOffer_WhenExists()
        {
            var offer = new Offer { Id = 1, Title = "Milk", Price = 2.5m };
            _offerRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(offer);

            var result = await _offerService.GetByIdAsync(1, default);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Title.Should().Be("Milk");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenNotFound()
        {
            _offerRepoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Offer?)null);

            var act = async () => await _offerService.GetByIdAsync(99, default);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Offer by ID:99 not found.");
        }

        [Fact]
        public async Task CreateAsync_ShouldAddOffer()
        {
            var dto = new CreateOfferDTO { Title = "Bread", Price = 1.5m, Quantity = 10 };

            _offerRepoMock.Setup(r => r.AddAsync(It.IsAny<Offer>(), default)).Returns(Task.CompletedTask);

            await _offerService.CreateAsync(dto, default);

            _offerRepoMock.Verify(r => r.AddAsync(It.IsAny<Offer>(), default), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldRemoveOffer_WhenExists()
        {
            var offer = new Offer { Id = 5 };
            _offerRepoMock.Setup(r => r.GetByIdAsync(5, default)).ReturnsAsync(offer);

            await _offerService.DeleteByIdAsync(5, default);

            _offerRepoMock.Verify(r => r.Delete(offer), Times.Once);
        }

        [Fact]
        public async Task DecreaseAmountAsync_ShouldReduceQuantity_AndSetSoldOut()
        {
            var offer = new Offer { Id = 1, Quantity = 2, OfferStatus = OfferStatus.Active };
            _offerRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(offer);

            var result = await _offerService.DecreaseAmountAsync(1, 2, default);

            result.Success.Should().BeTrue();
            result.Message.Should().Be("Stock decreased successfully.");
            offer.Quantity.Should().Be(0);
            offer.OfferStatus.Should().Be(OfferStatus.SoldOut);
        }

        [Fact]
        public async Task DecreaseAmountAsync_ShouldFail_WhenNotEnoughQuantity()
        {
            var offer = new Offer { Id = 1, Quantity = 1 };
            _offerRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(offer);

            var result = await _offerService.DecreaseAmountAsync(1, 5, default);

            result.Success.Should().BeFalse();
            result.Message.Should().Be("Not enough quantity.");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList()
        {
            var offers = new List<Offer> { new Offer { Id = 1, Title = "Eggs" } };
            _offerRepoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(offers);

            var result = await _offerService.GetAllAsync(default);

            result.Should().HaveCount(1);
            result.First().Title.Should().Be("Eggs");
        }

        [Fact]
        public async Task GetPriceAsync_ShouldReturnCorrectPrice()
        {
            var offer = new Offer { Id = 1, Price = 3.5m };
            _offerRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(offer);

            var result = await _offerService.GetPriceAsync(1, default);

            result.Should().Be(3.5m);
        }

        [Fact]
        public async Task GetQuantityAsync_ShouldReturnCorrectQuantity()
        {
            var offer = new Offer { Id = 1, Quantity = 7 };
            _offerRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(offer);

            var result = await _offerService.GetQuantityAsync(1, default);

            result.Should().Be(7);
        }

        [Fact]
        public async Task GetAllActiveByCategoryIdAsync_ShouldReturnList()
        {
            var offers = new List<Offer> {
                new Offer { Id = 1, Title = "Eggs", OfferStatus = OfferStatus.Active, CategoryId = 2},
                new Offer { Id = 2, Title = "Milk", OfferStatus = OfferStatus.Expired, CategoryId = 2}
            };
            _offerRepoMock.Setup(r => r.GetAllActiveByCategoryIdWithImageAsync(2, default)).ReturnsAsync(offers);

            var result = await _offerService.GetAllActiveByCategoryIdAsync(2, default);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task ArchiveExpiredOffersAsync_ShouldSetOfferStatusToExpired()
        {
            var offers = new List<Offer> {
                new Offer { Id = 1, Title = "Eggs", OfferStatus = OfferStatus.Active, CategoryId = 2},
                new Offer { Id = 2, Title = "Milk", OfferStatus = OfferStatus.SoldOut, CategoryId = 2}
            };
            _offerRepoMock.Setup(r => r.GetExpiredButHavingActiveStatusOffersAsync(default)).ReturnsAsync(offers);

            await _offerService.ArchiveExpiredOffersAsync(default);

            offers.Should().AllSatisfy(o => o.OfferStatus.Should().Be(OfferStatus.Expired));
        }
    }
}
