using Microsoft.EntityFrameworkCore;
using OfferManagement.Application.Purchases.Models;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain.Entities;
using OfferManagement.Persistance.Context;

namespace OfferManagement.Infrastructure.Purchases
{
    public class PurchaseRepository : BaseRepository<Purchase>, IPurchaseRepository
    {
        public PurchaseRepository(OfferManagementContext context) : base(context) { }

        public async Task<IEnumerable<Purchase>> GetAllByOfferIdAsync(int offerId, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(x => x.OfferId == offerId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Purchase>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Purchase>> GetUserPurchasesAsync(int userId, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
