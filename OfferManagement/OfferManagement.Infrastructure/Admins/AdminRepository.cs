using Microsoft.EntityFrameworkCore;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Persistance.Context;

namespace OfferManagement.Infrastructure.Admins
{
    public class AdminRepository : BaseRepository<Administrator>, IAdminRepository
    {
        public AdminRepository(OfferManagementContext context) : base(context) { }

        public async Task<Administrator?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _dbSet.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
        }
    }
}
