using Moq;
using OfferManagement.Application.EntityServices.Offers;
using OfferManagement.Application.EntityServices.Purchases;
using OfferManagement.Application.Exceptions;
using OfferManagement.Application.Purchases.Models;
using OfferManagement.Application.Repositories;
using OfferManagement.Application.Users;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Tests.PurchaseTests
{
    public class PurchaseServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IOfferService> _offerServiceMock;
        private readonly Mock<IPurchaseRepository> _purchaseRepositoryMock;
        private readonly PurchaseService _purchaseService;

        public PurchaseServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<IUserService>();
            _offerServiceMock = new Mock<IOfferService>();
            _purchaseRepositoryMock = new Mock<IPurchaseRepository>();

            _unitOfWorkMock.Setup(u => u.PurchaseRepository).Returns(_purchaseRepositoryMock.Object);

            _purchaseService = new PurchaseService(_unitOfWorkMock.Object, _userServiceMock.Object, _offerServiceMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsPurchaseDTO()
        {
            var purchase = new Purchase { Id = 1, OfferId = 10 };
            _purchaseRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(purchase);

            var result = await _purchaseService.GetByIdAsync(1, CancellationToken.None);

            Assert.Equal(1, result.Id);
            Assert.Equal(10, result.OfferId);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ThrowsNotFoundException()
        {
            _purchaseRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Purchase?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseService.GetByIdAsync(1, CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllPurchases()
        {
            var purchases = new List<Purchase> { new Purchase { Id = 1 }, new Purchase { Id = 2 } };
            _purchaseRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(purchases);

            var result = await _purchaseService.GetAllAsync(CancellationToken.None);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_EmptyList_ThrowsNotFoundException()
        {
            _purchaseRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Purchase>());

            var result = await _purchaseService.GetAllAsync(CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllByOfferIdAsync_ExistingOfferId_ReturnsPurchases()
        {
            var purchases = new List<Purchase> { new Purchase { Id = 1, OfferId = 5 } };
            _purchaseRepositoryMock.Setup(repo => repo.GetAllByOfferIdAsync(5, default)).ReturnsAsync(purchases);

            var result = await _purchaseService.GetAllByOfferIdAsync(5, CancellationToken.None);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllByOfferIdAsync_NotFound_ReturnsEmptyList()
        {
            _purchaseRepositoryMock.Setup(repo => repo.GetAllByOfferIdAsync(It.IsAny<int>(), default)).ReturnsAsync(new List<Purchase>());

            var result = await _purchaseService.GetAllByOfferIdAsync(123, CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllByUserIdAsync_ReturnsPurchases()
        {
            var purchases = new List<Purchase> { new Purchase { Id = 1, UserId = 10 } };
            _purchaseRepositoryMock.Setup(repo => repo.GetAllByUserIdAsync(10, CancellationToken.None)).ReturnsAsync(purchases);

            var result = await _purchaseService.GetAllByUserIdAsync(10, CancellationToken.None);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllByUserIdAsync_Empty_ReturnsEmptyList()
        {
            _purchaseRepositoryMock.Setup(repo => repo.GetAllByUserIdAsync(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync(new List<Purchase>());

            var result = await _purchaseService.GetAllByUserIdAsync(123, CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_AddsPurchase_AndCommits()
        {
            var dto = new CreatePurchaseDTO { OfferId = 1, UserId = 2 };

            await _purchaseService.CreateAsync(dto, CancellationToken.None);

            _purchaseRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Purchase>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ExistingPurchase_DeletesAndCommits()
        {
            var purchase = new Purchase { Id = 1 };
            _purchaseRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(purchase);

            await _purchaseService.DeleteByIdAsync(1, CancellationToken.None);

            _purchaseRepositoryMock.Verify(repo => repo.Delete(purchase), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_NotFound_ThrowsException()
        {
            _purchaseRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Purchase?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseService.DeleteByIdAsync(99, CancellationToken.None));
        }

    }
}
