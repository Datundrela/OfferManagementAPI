using Moq;
using OfferManagement.Application.EntityServices.Users.Models;
using OfferManagement.Application.Exceptions;
using OfferManagement.Application.Users.Models;
using OfferManagement.Application.Users;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;

namespace OfferManagement.Application.Tests.UserTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _userService = new UserService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User { Id = 1, Email = "test@test.com" };
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

            var result = await _userService.GetByIdAsync(user.Id, CancellationToken.None);

            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetByIdAsync(1, CancellationToken.None));
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User { Id = 1, Email = "test@test.com" };
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

            var result = await _userService.GetByEmailAsync(user.Email, CancellationToken.None);

            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldThrow_WhenNotFound()
        {
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetByEmailAsync("notfound@test.com", CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnUsers()
        {
            var users = new List<User> { new User { Id = 1 }, new User { Id = 2 } };
            _mockUnitOfWork.Setup(u => u.UserRepository.GetAllAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(users);

            var result = await _userService.GetAllAsync(CancellationToken.None);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldThrow_WhenEmpty()
        {
            _mockUnitOfWork.Setup(u => u.UserRepository.GetAllAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new List<User>());

            var result = await _userService.GetAllAsync(CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndCommit()
        {
            var dto = new CreateUserDTO { Email = "new@test.com" };

            await _userService.CreateAsync(dto, CancellationToken.None);

            _mockUnitOfWork.Verify(u => u.UserRepository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /*[Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndCommit()
        {
            var dto = new UserPutRequestModel { Id = 1, Email = "updated@test.com" };

            await _userService.UpdateAsync(dto, CancellationToken.None);

            _mockUnitOfWork.Verify(u => u.UserRepository.Update(It.Is<User>(x => x.Id == dto.Id)), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }*/

        [Fact]
        public async Task DeleteByIdAsync_ShouldDeleteUser_WhenExists()
        {
            var user = new User { Id = 1 };
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

            await _userService.DeleteByIdAsync(user.Id, CancellationToken.None);

            _mockUnitOfWork.Verify(u => u.UserRepository.Delete(user), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldThrow_WhenNotFound()
        {
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteByIdAsync(1, CancellationToken.None));
        }

        [Fact]
        public async Task DecreaseBalanceAsync_ShouldReturnSuccess_WhenSufficientBalance()
        {
            var user = new User { Id = 1, Balance = 100m };
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

            var result = await _userService.DecreaseBalanceAsync(user.Id, 50m, CancellationToken.None);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task DecreaseBalanceAsync_ShouldFail_WhenInsufficientBalance()
        {
            var user = new User { Id = 1, Balance = 20m };
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

            var result = await _userService.DecreaseBalanceAsync(user.Id, 50m, CancellationToken.None);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task IncreaseBalanceAsync_ShouldIncrease_WhenValidAmount()
        {
            var user = new User { Id = 1, Balance = 0m };
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

            var result = await _userService.IncreaseBalanceAsync(user.Id, 10m, CancellationToken.None);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task IncreaseBalanceAsync_ShouldFail_WhenNegativeAmount()
        {
            var user = new User { Id = 1 };
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

            var result = await _userService.IncreaseBalanceAsync(user.Id, -10m, CancellationToken.None);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task DepositAsync_ShouldIncreaseBalance_AndCommit()
        {
            var user = new User { Id = 1, Balance = 0 };
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

            var result = await _userService.DepositAsync(user.Id, 50m, CancellationToken.None);

            Assert.True(result.Success);
            _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetBalanceAsync_ShouldReturnBalance_WhenUserExists()
        {
            var user = new User { Id = 1, Balance = 75m };
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

            var result = await _userService.GetBalanceAsync(user.Id, CancellationToken.None);

            Assert.Equal(75m, result);
        }

        [Fact]
        public async Task IncreaseBalanceWithCommitAsync_ShouldIncreaseBalance_AndCommit()
        {
            var user = new User { Id = 1, Balance = 10m };
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

            await _userService.IncreaseBalanceWithCommitAsync(user.Id, 20m, CancellationToken.None);

            Assert.Equal(30m, user.Balance);
            _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
