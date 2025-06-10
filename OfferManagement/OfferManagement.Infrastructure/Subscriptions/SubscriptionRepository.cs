using OfferManagement.Domain.Entities;
using OfferManagement.Application;
using OfferManagement.Application.Repositories;
using OfferManagement.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace OfferManagement.Infrastructure.Subscriptions
{
    public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(OfferManagementContext context) : base(context) { }

        public async Task<IEnumerable<Subscription>> GetAllByCategoryIdAsync(int categoryId, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(x => x.CategoryId == categoryId).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Subscription>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        }

        public async Task<Subscription?> GetByUserIdAndCategoryId(int userId, int categoryId, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(s => s.UserId == userId & s.CategoryId == categoryId).SingleOrDefaultAsync(cancellationToken);
        }
    }
}
