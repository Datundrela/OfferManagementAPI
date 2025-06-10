using FluentAssertions;
using Moq;
using OfferManagement.Application.EntityServices.Companies.Models;
using OfferManagement.Application.EntityServices.Companies;
using OfferManagement.Application.Exceptions;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Tests.CompanyTests
{
    public class CompanyServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICompanyRepository> _companyRepositoryMock;
        private readonly CompanyService _companyService;

        public CompanyServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _companyRepositoryMock = new Mock<ICompanyRepository>();

            _unitOfWorkMock.Setup(u => u.CompanyRepository).Returns(_companyRepositoryMock.Object);

            _companyService = new CompanyService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCompanyDTO_WhenExists()
        {
            var company = new Company { Id = 1, Name = "Tech Inc", Email = "info@tech.com" };

            _companyRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);

            var result = await _companyService.GetByIdAsync(1, CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Tech Inc");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenNotExists()
        {
            _companyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company)null);

            var act = async () => await _companyService.GetByIdAsync(404, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnCompanyDTO_WhenExists()
        {
            var company = new Company { Id = 1, Name = "Biz Co", Email = "contact@biz.com" };

            _companyRepositoryMock.Setup(r => r.GetByEmailAsync("contact@biz.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);

            var result = await _companyService.GetByEmailAsync("contact@biz.com", CancellationToken.None);

            result.Email.Should().Be("contact@biz.com");
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldThrowNotFoundException_WhenNotExists()
        {
            _companyRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company)null);

            var act = async () => await _companyService.GetByEmailAsync("notfound@mail.com", CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList_WhenExists()
        {
            var companies = new List<Company>
        {
            new Company { Id = 1, Name = "Company A" },
            new Company { Id = 2, Name = "Company B" }
        };

            _companyRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(companies);

            var result = await _companyService.GetAllAsync(CancellationToken.None);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenEmpty()
        {
            _companyRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Company>());

            var result = await _companyService.GetAllAsync(CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldAdd()
        {
            var createDto = new CreateCompanyDTO { Name = "NewCo", Email = "new@co.com" };

            _companyRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _companyService.CreateAsync(createDto, CancellationToken.None);

            _companyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldDelete_WhenExists()
        {
            var company = new Company { Id = 3, Name = "DelCo" };

            _companyRepositoryMock.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);

            await _companyService.DeleteByIdAsync(3, CancellationToken.None);

            _companyRepositoryMock.Verify(r => r.Delete(company), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldThrowNotFoundException_WhenNotExists()
        {
            _companyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company)null);

            var act = async () => await _companyService.DeleteByIdAsync(404, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task ActivateAsync_ShouldSetIsActiveTrue_AndCommit()
        {
            var company = new Company { Id = 5, IsActive = false };

            _companyRepositoryMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);

            await _companyService.ActivateAsync(5, CancellationToken.None);

            company.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task DeactivateAsync_ShouldSetIsActiveFalse_AndCommit()
        {
            var company = new Company { Id = 6, IsActive = true };

            _companyRepositoryMock.Setup(r => r.GetByIdAsync(6, It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);

            await _companyService.DeactivateAsync(6, CancellationToken.None);

            company.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task ActivateAsync_ShouldThrowNotFoundException_WhenCompanyNotExists()
        {
            _companyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company)null);

            var act = async () => await _companyService.ActivateAsync(999, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeactivateAsync_ShouldThrowNotFoundException_WhenCompanyNotExists()
        {
            _companyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company)null);

            var act = async () => await _companyService.DeactivateAsync(999, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
