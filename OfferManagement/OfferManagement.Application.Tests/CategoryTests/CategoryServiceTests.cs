using FluentAssertions;
using Moq;
using OfferManagement.Application.EntityServices.Categories.Models;
using OfferManagement.Application.EntityServices.Categories;
using OfferManagement.Application.Exceptions;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Tests.CategoryTests
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();

            _unitOfWorkMock.Setup(u => u.CategoryRepository).Returns(_categoryRepositoryMock.Object);

            _categoryService = new CategoryService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCategoryDTO_WhenExists()
        {
            var category = new Category { Id = 1, Name = "Food" };
            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var result = await _categoryService.GetByIdAsync(1, CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Food");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenNotExists()
        {
            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category)null);

            var act = async () => await _categoryService.GetByIdAsync(42, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList_WhenExists()
        {
            var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Food" },
            new Category { Id = 2, Name = "Drinks" }
        };

            _categoryRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            var result = await _categoryService.GetAllAsync(CancellationToken.None);

            result.Should().HaveCount(2);
            result.Select(x => x.Name).Should().Contain(new[] { "Food", "Drinks" });
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenEmpty()
        {
            _categoryRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Category>());

            var result = await _categoryService.GetAllAsync(CancellationToken.None);

            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldAddAndCommit()
        {
            var createDto = new CreateCategoryDTO { Name = "Electronics" };

            _categoryRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _categoryService.CreateCategoryAsync(createDto, CancellationToken.None);

            _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldDelete_WhenExists()
        {
            var category = new Category { Id = 1, Name = "Snacks" };

            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()));

            await _categoryService.DeleteByIdAsync(1, CancellationToken.None);

            _categoryRepositoryMock.Verify(r => r.Delete(category), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldThrowNotFoundException_WhenNotExists()
        {
            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category)null);

            var act = async () => await _categoryService.DeleteByIdAsync(99, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
