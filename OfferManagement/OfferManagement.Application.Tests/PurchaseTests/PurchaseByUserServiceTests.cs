using Moq;
using OfferManagement.Application.EntityServices.Offers;
using OfferManagement.Application.EntityServices.Purchases;
using OfferManagement.Application.EntityServices.Purchases.Models;
using OfferManagement.Application.EntityServices.Users.Models;
using OfferManagement.Application.Users;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Tests.PurchaseTests
{
    public class PurchaseByUserServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock; 
        private readonly Mock<IUserService> _userServiceMock; 
        private readonly Mock<IOfferService> _offerServiceMock; 
        private readonly PurchaseByUserService _service;

        public PurchaseByUserServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<IUserService>();
            _offerServiceMock = new Mock<IOfferService>();

            _service = new PurchaseByUserService(_unitOfWorkMock.Object, _userServiceMock.Object, _offerServiceMock.Object);
        }

        [Fact]
        public async Task PurchaseOfferAsync_SuccessfulPurchase_ReturnsSuccess()
        {
            var request = new PurchaseOfferRequestModel { OfferId = 1, UserId = 1, Quantity = 2 };
            var cancellationToken = CancellationToken.None;
            decimal price = 10;

            _offerServiceMock.Setup(x => x.GetPriceAsync(1, cancellationToken)).ReturnsAsync(price);
            _userServiceMock.Setup(x => x.GetBalanceAsync(1, cancellationToken)).ReturnsAsync(50);
            _offerServiceMock.Setup(x => x.GetQuantityAsync(1, cancellationToken)).ReturnsAsync(5);

            _unitOfWorkMock.Setup(x => x.PurchaseRepository.AddAsync(It.IsAny<Purchase>(), cancellationToken)).Returns(Task.CompletedTask);
            _offerServiceMock.Setup(x => x.DecreaseAmountAsync(1, 2, cancellationToken));
            _userServiceMock.Setup(x => x.DecreaseBalanceAsync(1, 20, cancellationToken));
            _unitOfWorkMock.Setup(x => x.CommitAsync(cancellationToken));
            _unitOfWorkMock.Setup(x => x.CommitTransactionAsync(cancellationToken)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(cancellationToken)).Returns(Task.CompletedTask);

            var result = await _service.PurchaseOfferAsync(request, cancellationToken);

            Assert.True(result.Success);
            Assert.Equal("Purchase Done Successfully.", result.Message);
        }

        [Fact]
        public async Task PurchaseOfferAsync_InsufficientBalance_ReturnsFailure()
        {
            var request = new PurchaseOfferRequestModel { OfferId = 1, UserId = 1, Quantity = 2 };
            var cancellationToken = CancellationToken.None;

            _offerServiceMock.Setup(x => x.GetPriceAsync(1, cancellationToken)).ReturnsAsync(10);
            _userServiceMock.Setup(x => x.GetBalanceAsync(1, cancellationToken)).ReturnsAsync(5);

            var result = await _service.PurchaseOfferAsync(request, cancellationToken);

            Assert.False(result.Success);
            Assert.Equal("Insufficient Funds.", result.Message);
        }

        [Fact]
        public async Task PurchaseOfferAsync_NotEnoughStock_ReturnsFailure()
        {
            var request = new PurchaseOfferRequestModel { OfferId = 1, UserId = 1, Quantity = 10 };
            var cancellationToken = CancellationToken.None;

            _offerServiceMock.Setup(x => x.GetPriceAsync(1, cancellationToken)).ReturnsAsync(10);
            _userServiceMock.Setup(x => x.GetBalanceAsync(1, cancellationToken)).ReturnsAsync(500);
            _offerServiceMock.Setup(x => x.GetQuantityAsync(1, cancellationToken)).ReturnsAsync(5);

            var result = await _service.PurchaseOfferAsync(request, cancellationToken);

            Assert.False(result.Success);
            Assert.Equal("Not enough items in the stock", result.Message);
        }

        [Fact]
        public async Task CancelPurchaseAsync_Valid_ReturnsSuccess()
        {
            var cancellationToken = CancellationToken.None;
            var purchase = new Purchase { Id = 1, OfferId = 1, UserId = 1, TotalPrice = 100, Quantity = 1, PurchaseDate = DateTime.UtcNow };

            var offer = new Offer { Id = 1 };

            _unitOfWorkMock.Setup(x => x.PurchaseRepository.GetByIdAsync(1, cancellationToken)).ReturnsAsync(purchase);
            _unitOfWorkMock.Setup(x => x.OfferRepository.GetByIdAsync(1, cancellationToken)).ReturnsAsync(offer);
            _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(cancellationToken)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.CommitAsync(cancellationToken));
            _unitOfWorkMock.Setup(x => x.CommitTransactionAsync(cancellationToken)).Returns(Task.CompletedTask);

            _userServiceMock.Setup(x => x.IncreaseBalanceAsync(1, 100, cancellationToken));
            _offerServiceMock.Setup(x => x.IncreaseAmountAsync(1, 1, cancellationToken));

            var result = await _service.CancelPurchaseAsync(1, cancellationToken);

            Assert.True(result.Success);
            Assert.Equal("Purchase cancelled successfully.", result.Message);
        }

        [Fact]
        public async Task CancelPurchaseAsync_ExpiredTime_ReturnsFailure()
        {
            var cancellationToken = CancellationToken.None;
            var purchase = new Purchase
            {
                Id = 1,
                OfferId = 1,
                UserId = 1,
                TotalPrice = 100,
                Quantity = 1,
                PurchaseDate = DateTime.UtcNow.AddMinutes(-6)
            };

            _unitOfWorkMock.Setup(x => x.PurchaseRepository.GetByIdAsync(1, cancellationToken)).ReturnsAsync(purchase);

            var result = await _service.CancelPurchaseAsync(1, cancellationToken);

            Assert.False(result.Success);
            Assert.Equal("Time for refund has expired", result.Message);
        }

        [Fact]
        public async Task CancelPurchaseForCancelledOfferAsync_RefundsCorrectly()
        {
            var cancellationToken = CancellationToken.None;
            var purchase = new Purchase { Id = 1, OfferId = 2, UserId = 3, TotalPrice = 50, Quantity = 1 };

            _userServiceMock.Setup(x => x.IncreaseBalanceAsync(3, 50, cancellationToken));
            _offerServiceMock.Setup(x => x.IncreaseAmountAsync(2, 1, cancellationToken));

            await _service.CancelPurchaseForCancelledOfferAsync(purchase, cancellationToken);

            _unitOfWorkMock.Verify(x => x.PurchaseRepository.Delete(purchase), Times.Once);
        }

        [Fact]
        public async Task CancelPurchaseForCancelledOfferAsync_AlreadyRefunded_DoesNothing()
        {
            var cancellationToken = CancellationToken.None;
            var purchase = new Purchase { Id = 1, IsRefunded = true };

            await _service.CancelPurchaseForCancelledOfferAsync(purchase, cancellationToken);

            _userServiceMock.Verify(x => x.IncreaseBalanceAsync(It.IsAny<int>(), It.IsAny<decimal>(), cancellationToken), Times.Never);
        }
    }
}
