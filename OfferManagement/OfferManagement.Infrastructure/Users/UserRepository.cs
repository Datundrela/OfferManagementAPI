using Microsoft.EntityFrameworkCore;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Persistance.Context;

namespace OfferManagement.Infrastructure.Users
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(OfferManagementContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _dbSet.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
        }
    }
}
