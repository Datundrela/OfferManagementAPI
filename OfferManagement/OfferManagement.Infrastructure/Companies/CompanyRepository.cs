using Microsoft.EntityFrameworkCore;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.Categories;
using OfferManagement.Persistance.Context;

namespace OfferManagement.Infrastructure.Companies
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(OfferManagementContext context) : base(context) { }

        public async Task<Company?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _dbSet.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<bool> IsActiveAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(x => x.Id == id)
                       .Select(x => x.IsActive)
                       .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
