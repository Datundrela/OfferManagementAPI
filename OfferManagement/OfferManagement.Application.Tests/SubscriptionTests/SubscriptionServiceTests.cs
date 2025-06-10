using Moq;
using OfferManagement.Application.EntityServices.Subscriptions.Models;
using OfferManagement.Application.EntityServices.Subscriptions;
using OfferManagement.Application.Exceptions;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Tests.SubscriptionTests
{
    public class SubscriptionServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ISubscriptionRepository> _subscriptionRepoMock;
        private readonly SubscriptionService _service;

        public SubscriptionServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _subscriptionRepoMock = new Mock<ISubscriptionRepository>();

            _unitOfWorkMock.Setup(x => x.SubscriptionRepository).Returns(_subscriptionRepoMock.Object);
            _service = new SubscriptionService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSubscriptionDTO_WhenFound()
        {
            var subscription = new Subscription { Id = 1, CategoryId = 2, UserId = 3 };
            var cancellationToken = CancellationToken.None;

            _subscriptionRepoMock.Setup(x => x.GetByIdAsync(1, cancellationToken)).ReturnsAsync(subscription);

            var result = await _service.GetByIdAsync(1, cancellationToken);

            Assert.NotNull(result);
            Assert.Equal(subscription.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenNotFound()
        {
            var cancellationToken = CancellationToken.None;
            _subscriptionRepoMock.Setup(x => x.GetByIdAsync(1, cancellationToken)).ReturnsAsync((Subscription?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(1, cancellationToken));
        }

        [Fact]
        public async Task GetAllByCategoryIdAsync_ShouldReturnList_WhenFound()
        {
            var subscriptions = new List<Subscription> { new Subscription { Id = 1, CategoryId = 1, UserId = 2 } };
            var cancellationToken = CancellationToken.None;

            _subscriptionRepoMock.Setup(x => x.GetAllByCategoryIdAsync(1, cancellationToken)).ReturnsAsync(subscriptions);

            var result = await _service.GetAllByCategoryIdAsync(1, cancellationToken);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetAllByCategoryIdAsync_ShouldReturnEmptyList_WhenNoneFound()
        {
            var cancellationToken = CancellationToken.None;

            _subscriptionRepoMock.Setup(x => x.GetAllByCategoryIdAsync(1, cancellationToken)).ReturnsAsync(new List<Subscription>());

            var result = await _service.GetAllByCategoryIdAsync(1, cancellationToken);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllByUserIdAsync_ShouldReturnList_WhenFound()
        {
            var subscriptions = new List<Subscription> { new Subscription { Id = 1, CategoryId = 1, UserId = 2 } };
            var cancellationToken = CancellationToken.None;

            _subscriptionRepoMock.Setup(x => x.GetAllByUserIdAsync(2, cancellationToken)).ReturnsAsync(subscriptions);

            var result = await _service.GetAllByUserIdAsync(2, cancellationToken);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetAllByUserIdAsync_ShouldReturnEmptyList_WhenNoneFound()
        {
            var cancellationToken = CancellationToken.None;

            _subscriptionRepoMock.Setup(x => x.GetAllByUserIdAsync(2, cancellationToken)).ReturnsAsync(new List<Subscription>());

            var result = await _service.GetAllByUserIdAsync(2, cancellationToken);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList_WhenFound()
        {
            var subscriptions = new List<Subscription> { new Subscription { Id = 1 } };
            var cancellationToken = CancellationToken.None;

            _subscriptionRepoMock.Setup(x => x.GetAllAsync(cancellationToken)).ReturnsAsync(subscriptions);

            var result = await _service.GetAllAsync(cancellationToken);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoneFound()
        {
            var cancellationToken = CancellationToken.None;

            _subscriptionRepoMock.Setup(x => x.GetAllAsync(cancellationToken)).ReturnsAsync(new List<Subscription>());

            var result = await _service.GetAllAsync(cancellationToken);

            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndCommit()
        {
            var dto = new CreateSubscriptionDTO { UserId = 1, CategoryId = 2 };
            var cancellationToken = CancellationToken.None;

            _subscriptionRepoMock.Setup(x => x.AddAsync(It.IsAny<Subscription>(), cancellationToken)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.CommitAsync(CancellationToken.None));

            await _service.CreateAsync(dto, cancellationToken);

            _subscriptionRepoMock.Verify(x => x.AddAsync(It.IsAny<Subscription>(), cancellationToken), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldCallDeleteAndCommit_WhenFound()
        {
            var subscription = new Subscription { Id = 1 };
            var cancellationToken = CancellationToken.None;

            _subscriptionRepoMock.Setup(x => x.GetByIdAsync(1, cancellationToken)).ReturnsAsync(subscription);
            _unitOfWorkMock.Setup(x => x.CommitAsync(CancellationToken.None));

            await _service.DeleteByIdAsync(1, cancellationToken);

            _subscriptionRepoMock.Verify(x => x.Delete(subscription), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldThrowNotFoundException_WhenNotFound()
        {
            var cancellationToken = CancellationToken.None;

            _subscriptionRepoMock.Setup(x => x.GetByIdAsync(1, cancellationToken)).ReturnsAsync((Subscription?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteByIdAsync(1, cancellationToken));
        }
    }
}
