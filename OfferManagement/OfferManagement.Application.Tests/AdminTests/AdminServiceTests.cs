using FluentAssertions;
using Moq;
using OfferManagement.Application.EntityServices.Admins.Models;
using OfferManagement.Application.EntityServices.Admins;
using OfferManagement.Application.Exceptions;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;

namespace OfferManagement.Application.Tests.AdminTests
{
    public class AdminServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IAdminRepository> _adminRepositoryMock;
        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _adminRepositoryMock = new Mock<IAdminRepository>();

            _unitOfWorkMock.Setup(u => u.AdminRepository).Returns(_adminRepositoryMock.Object);

            _adminService = new AdminService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnAdminDTO_WhenAdminExists()
        {
            // Arrange
            var admin = new Administrator { Id = 1, Email = "admin@test.com" };
            _adminRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);

            // Act
            var result = await _adminService.GetByIdAsync(1, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Email.Should().Be("admin@test.com");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenAdminDoesNotExist()
        {
            _adminRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Administrator)null);

            var act = async () => await _adminService.GetByIdAsync(1, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAdmin()
        {
            var createDto = new CreateAdminDTO { Email = "new@admin.com"};

            _adminRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Administrator>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _adminService.CreateAsync(createDto, CancellationToken.None);

            _adminRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Administrator>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldDelete_WhenAdminExists()
        {
            var admin = new Administrator { Id = 5 };

            _adminRepositoryMock.Setup(repo => repo.GetByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);

            await _adminService.DeleteByIdAsync(5, CancellationToken.None);

            _adminRepositoryMock.Verify(repo => repo.Delete(admin), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList_WhenAdminsExist()
        {
            var admins = new List<Administrator> 
            {
                new Administrator { Id = 1, Email = "a1@mail.com" },
                new Administrator { Id = 2, Email = "a2@mail.com" }
            };


            _adminRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(admins);

            var result = await _adminService.GetAllAsync(CancellationToken.None);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoneExist()
        {
            _adminRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Administrator>());

            var result = await _adminService.GetAllAsync(CancellationToken.None);

            result.Should().HaveCount(0);
        }
    }
}
