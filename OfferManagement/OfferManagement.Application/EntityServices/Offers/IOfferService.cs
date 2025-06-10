using OfferManagement.Application.EntityServices.Offers.Models;
using OfferManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace OfferManagement.Application.EntityServices.Offers
{
    public interface IOfferService
    {
        Task<OfferDTO> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<OfferDTO>> GetAllByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
        Task<IEnumerable<OfferDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task CreateAsync(CreateOfferDTO offerDto, CancellationToken cancellationToken);
        Task DeleteByIdAsync(int id, CancellationToken cancellationToken);
        Task ArchiveExpiredOffersAsync(CancellationToken cancellationToken);
        Task<StockChangeResult> DecreaseAmountAsync(int offerId, int amount , CancellationToken cancellationToken);
        Task<StockChangeResult> IncreaseAmountAsync(int offerId, int amount, CancellationToken cancellationToken);
        Task<decimal> GetPriceAsync(int offerId, CancellationToken cancellationToken);
        Task<int> GetQuantityAsync(int offerId, CancellationToken cancellationToken);
        Task<OfferStatus> GetStatusAsync(int offerId, CancellationToken cancellationToken);
        Task ChangeStatusAsync(int offerId, OfferStatus newStatus, CancellationToken cancellationToken);
        Task<IEnumerable<OfferDTO>> GetAllActiveByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
    }
}
