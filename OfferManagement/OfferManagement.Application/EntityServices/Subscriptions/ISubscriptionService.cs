using OfferManagement.Application.EntityServices.Subscriptions.Models;
using OfferManagement.Application.EntityServices.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Subscriptions
{
    public interface ISubscriptionService
    {
        Task<SubscriptionDTO> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<SubscriptionDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<SubscriptionDTO>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<SubscriptionDTO>> GetAllByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
        Task CreateAsync(CreateSubscriptionDTO subscriptionDto, CancellationToken cancellationToken);
        Task DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
