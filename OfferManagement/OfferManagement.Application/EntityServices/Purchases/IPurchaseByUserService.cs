using OfferManagement.Application.EntityServices.Purchases.Models;
using OfferManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Purchases
{
    public interface IPurchaseByUserService
    {
        Task<PurchaseOfferResponseModel> PurchaseOfferAsync(PurchaseOfferRequestModel purchaseOfferModel, CancellationToken cancellationToken);
        Task<PurchaseOfferResponseModel> CancelPurchaseAsync(int purchaseId, CancellationToken cancellationToken);
        Task CancelPurchaseForCancelledOfferAsync(Purchase purchase, CancellationToken cancellationToken);
    }
}
