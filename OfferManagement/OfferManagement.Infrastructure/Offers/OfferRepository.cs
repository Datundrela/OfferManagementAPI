using Microsoft.EntityFrameworkCore;
using OfferManagement.Application.Repositories;
using OfferManagement.Domain;
using OfferManagement.Domain.Entities;
using OfferManagement.Persistance.Context;

namespace OfferManagement.Infrastructure.Offers
{
    public class OfferRepository : BaseRepository<Offer>, IOfferRepository
    {
        public OfferRepository(OfferManagementContext context) : base(context) { }

        public async Task<IEnumerable<Offer>> GetActiveOffersAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.Where(x => x.OfferStatus == OfferStatus.Active).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Offer>> GetOffersByCategoryIdAsync(int categoryId, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(x => x.CategoryId == categoryId)
                               .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Offer>> GetExpiredButHavingActiveStatusOffersAsync(CancellationToken cancellationToken)
        {
            var expiredOffers = await _dbSet
            .Where(o => o.ExpiryDate <= DateTime.UtcNow & o.OfferStatus != OfferStatus.Expired)
            .ToListAsync();

            return expiredOffers;
        }

        public async Task<List<Offer>> GetAllActiveByCategoryIdWithImageAsync(int categoryId, CancellationToken cancellationToken)
        {
            var activeOffers = await _dbSet.Where(x => x.CategoryId == categoryId & x.OfferStatus == OfferStatus.Active).Include(o => o.Image).ToListAsync(cancellationToken);

            return activeOffers;
        }

        public async Task<List<Offer>> GetOffersByCompanyIdAsync(int companyId, CancellationToken cancellationToken)
        {
            List<Offer> offers = await GetAllByPredicateQuery(o => o.CompanyId == companyId).Include(o => o.Image).ToListAsync(cancellationToken);

            return offers;
        }
    }
}
