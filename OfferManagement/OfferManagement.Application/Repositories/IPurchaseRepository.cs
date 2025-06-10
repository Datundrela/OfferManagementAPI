using OfferManagement.Application.Purchases.Models;
using OfferManagement.Domain.Entities;
using System.Linq.Expressions;

namespace OfferManagement.Application.Repositories
{
    public interface IPurchaseRepository : IRepository<Purchase>
    {
        Task<IEnumerable<Purchase>> GetUserPurchasesAsync(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<Purchase>> GetAllByOfferIdAsync(int offerId, CancellationToken cancellationToken);
        Task<IEnumerable<Purchase>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken);
    }
}
