using Moq;
using OfferManagement.Application.EntityServices.Subscriptions.Models;
using OfferManagement.Application.EntityServices.Subscriptions;
using OfferManagement.Application.Exceptions;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using System.Linq.Expressions;

namespace OfferManagement.Application.Tests.SubscriptionTests
{
    public class SubscriptionByUserServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly Mock<ISubscriptionRepository> _subscriptionRepoMock;
        private readonly SubscriptionByUserService _service;

        public SubscriptionByUserServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepoMock = new Mock<IUserRepository>();
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _subscriptionRepoMock = new Mock<ISubscriptionRepository>();

            _unitOfWorkMock.Setup(x => x.UserRepository).Returns(_userRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.CategoryRepository).Returns(_categoryRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.SubscriptionRepository).Returns(_subscriptionRepoMock.Object);

            _service = new SubscriptionByUserService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task SubscribeToCategoryAsync_ShouldReturnSuccessTrue_WhenValid()
        {
            var userId = 1;
            var categoryId = 2;
            var cancellationToken = CancellationToken.None;
            var model = new SubscribeToCategoryRequestModel { CategoryId = categoryId };

            _userRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken)).ReturnsAsync(true);
            _categoryRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), cancellationToken)).ReturnsAsync(true);
            _subscriptionRepoMock.Setup(x => x.GetByUserIdAndCategoryId(userId, categoryId, cancellationToken)).ReturnsAsync((Subscription?)null);
            _subscriptionRepoMock.Setup(x => x.AddAsync(It.IsAny<Subscription>(), cancellationToken)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.CommitAsync(cancellationToken)).Returns(Task.CompletedTask);

            var result = await _service.SubscribeToCategoryAsync(userId, model, cancellationToken);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task SubscribeToCategoryAsync_ShouldReturnAlreadySubscribed_WhenExists()
        {
            var userId = 1;
            var categoryId = 2;
            var cancellationToken = CancellationToken.None;
            var model = new SubscribeToCategoryRequestModel { CategoryId = categoryId };

            _userRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken)).ReturnsAsync(true);
            _categoryRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), cancellationToken)).ReturnsAsync(true);
            _subscriptionRepoMock.Setup(x => x.GetByUserIdAndCategoryId(userId, categoryId, cancellationToken)).ReturnsAsync(new Subscription());

            var result = await _service.SubscribeToCategoryAsync(userId, model, cancellationToken);

            Assert.False(result.Success);
            Assert.Equal("Already subscribed", result.Message);
        }

        [Fact]
        public async Task SubscribeToCategoryAsync_ShouldThrow_WhenUserNotFound()
        {
            var cancellationToken = CancellationToken.None;
            var model = new SubscribeToCategoryRequestModel { CategoryId = 1 };

            _userRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken)).ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.SubscribeToCategoryAsync(1, model, cancellationToken));
        }

        [Fact]
        public async Task SubscribeToCategoryAsync_ShouldThrow_WhenCategoryNotFound()
        {
            var cancellationToken = CancellationToken.None;
            var model = new SubscribeToCategoryRequestModel { CategoryId = 1 };

            _userRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken)).ReturnsAsync(true);
            _categoryRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), cancellationToken)).ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.SubscribeToCategoryAsync(1, model, cancellationToken));
        }

        [Fact]
        public async Task UnsubscribeFromCategoryAsync_ShouldReturnSuccessTrue_WhenValid()
        {
            var userId = 1;
            var categoryId = 2;
            var cancellationToken = CancellationToken.None;
            var subscription = new Subscription { Id = 123 };

            _userRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken)).ReturnsAsync(true);
            _categoryRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), cancellationToken)).ReturnsAsync(true);
            _subscriptionRepoMock.Setup(x => x.GetByUserIdAndCategoryId(userId, categoryId, cancellationToken)).ReturnsAsync(subscription);
            _unitOfWorkMock.Setup(x => x.CommitAsync(cancellationToken));

            var result = await _service.UnsubscribeFromCategoryAsync(userId, categoryId, cancellationToken);

            Assert.True(result.Success);
            _subscriptionRepoMock.Verify(x => x.Delete(subscription), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task UnsubscribeFromCategoryAsync_ShouldThrow_WhenSubscriptionNotFound()
        {
            var userId = 1;
            var categoryId = 2;
            var cancellationToken = CancellationToken.None;

            _userRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken)).ReturnsAsync(true);
            _categoryRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), cancellationToken)).ReturnsAsync(true);
            _subscriptionRepoMock.Setup(x => x.GetByUserIdAndCategoryId(userId, categoryId, cancellationToken)).ReturnsAsync((Subscription?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.UnsubscribeFromCategoryAsync(userId, categoryId, cancellationToken));
        }

        [Fact]
        public async Task UnsubscribeFromCategoryAsync_ShouldThrow_WhenUserNotFound()
        {
            var cancellationToken = CancellationToken.None;

            _userRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken)).ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.UnsubscribeFromCategoryAsync(1, 2, cancellationToken));
        }

        [Fact]
        public async Task UnsubscribeFromCategoryAsync_ShouldThrow_WhenCategoryNotFound()
        {
            var cancellationToken = CancellationToken.None;

            _userRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken)).ReturnsAsync(true);
            _categoryRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), cancellationToken)).ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.UnsubscribeFromCategoryAsync(1, 2, cancellationToken));
        }
    }
}
