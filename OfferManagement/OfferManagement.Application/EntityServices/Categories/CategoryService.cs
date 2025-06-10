using Mapster;
using OfferManagement.Application.EntityServices.Categories.Models;
using OfferManagement.Application.EntityServices.Companies.Models;
using OfferManagement.Application.Exceptions;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryDTO> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            Category? category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null) throw new NotFoundException($"Category by ID:{id} not found.");

            return category.Adapt<CategoryDTO>();
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Category>? categories = await _unitOfWork.CategoryRepository.GetAllAsync(cancellationToken);
            if (!categories.Any()) return new List<CategoryDTO>();

            return categories.Adapt<IEnumerable<CategoryDTO>>();
        }

        public async Task CreateCategoryAsync(CreateCategoryDTO categoryDto, CancellationToken cancellationToken)
        {
            await _unitOfWork.CategoryRepository.AddAsync(categoryDto.Adapt<Category>(), cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            Category? category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null) throw new NotFoundException($"Category by ID:{id} not found.");

            _unitOfWork.CategoryRepository.Delete(category);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
