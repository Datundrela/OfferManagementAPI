using OfferManagement.Application.EntityServices.Purchases.Models;
using OfferManagement.Application.EntityServices.Users.Models;
using OfferManagement.Application.Purchases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Purchases
{
    public interface IPurchaseService
    {
        Task<PurchaseDTO> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<PurchaseDTO>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<PurchaseDTO>> GetAllByOfferIdAsync(int offerId, CancellationToken cancellationToken);
        Task<IEnumerable<PurchaseDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task CreateAsync(CreatePurchaseDTO purchaseDto, CancellationToken cancellationToken);
        Task DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
