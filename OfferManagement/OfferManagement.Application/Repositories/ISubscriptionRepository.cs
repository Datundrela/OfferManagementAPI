using OfferManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Repositories
{
    public interface ISubscriptionRepository : IRepository<Subscription>
    {
        Task<IEnumerable<Subscription>> GetAllByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
        Task<IEnumerable<Subscription>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<Subscription?> GetByUserIdAndCategoryId(int userId, int categoryId, CancellationToken cancellationToken);
    }
}
