using OfferManagement.Application.EntityServices.Categories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Categories
{
    public interface ICategoryService
    {
        Task<CategoryDTO> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<CategoryDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task CreateCategoryAsync(CreateCategoryDTO categoryDto, CancellationToken cancellationToken);
        Task DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
