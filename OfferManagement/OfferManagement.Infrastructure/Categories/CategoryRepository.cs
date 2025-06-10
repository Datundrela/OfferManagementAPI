using Microsoft.EntityFrameworkCore;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Persistance.Context;

namespace OfferManagement.Infrastructure.Categories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(OfferManagementContext context) : base(context) { }

        public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _dbSet.SingleOrDefaultAsync(x => x.Name == name, cancellationToken);
        }
    }
}
