using OfferManagement.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace OfferManagement.Application.Repositories
{
    public interface IOfferRepository : IRepository<Offer>
    {
        Task<IEnumerable<Offer>> GetActiveOffersAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Offer>> GetOffersByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
        Task<List<Offer>> GetOffersByCompanyIdAsync(int companyId, CancellationToken cancellationToken);
        Task<IEnumerable<Offer>> GetExpiredButHavingActiveStatusOffersAsync(CancellationToken cancellationToken);
        Task<List<Offer>> GetAllActiveByCategoryIdWithImageAsync(int categoryId, CancellationToken cancellationToken);

    }
}
