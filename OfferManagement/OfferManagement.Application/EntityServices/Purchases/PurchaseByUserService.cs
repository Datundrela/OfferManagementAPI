using OfferManagement.Application.EntityServices.Offers;
using OfferManagement.Application.EntityServices.Purchases.Models;
using OfferManagement.Application.Exceptions;
using OfferManagement.Application.Users;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using Serilog;

namespace OfferManagement.Application.EntityServices.Purchases
{
    public class PurchaseByUserService : IPurchaseByUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IOfferService _offerService;

        public PurchaseByUserService(IUnitOfWork unitOfWork, IUserService userService, IOfferService offerService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _offerService = offerService;
        }

        public async Task<PurchaseOfferResponseModel> PurchaseOfferAsync(PurchaseOfferRequestModel purchaseOfferModel, CancellationToken cancellationToken)
        {
            decimal totalPrice = purchaseOfferModel.Quantity * (await _offerService.GetPriceAsync(purchaseOfferModel.OfferId, cancellationToken));

            if (await _userService.GetBalanceAsync(purchaseOfferModel.UserId, cancellationToken) < totalPrice) return new PurchaseOfferResponseModel
            {
                Success = false,
                Message = "Insufficient Funds."
            };

            if (await _offerService.GetQuantityAsync(purchaseOfferModel.OfferId, cancellationToken) < purchaseOfferModel.Quantity) return new PurchaseOfferResponseModel
            {
                Success = false,
                Message = "Not enough items in the stock"
            };


            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                Purchase purchase = new Purchase
                {
                    OfferId = purchaseOfferModel.OfferId,
                    PurchaseDate = DateTime.UtcNow,
                    TotalPrice = totalPrice,
                    Quantity = purchaseOfferModel.Quantity,
                    UserId = purchaseOfferModel.UserId
                };

                await _unitOfWork.PurchaseRepository.AddAsync(purchase, cancellationToken);

                await _offerService.DecreaseAmountAsync(purchaseOfferModel.OfferId, purchaseOfferModel.Quantity, cancellationToken);

                if (await _offerService.GetQuantityAsync(purchaseOfferModel.OfferId, cancellationToken) == 0)
                {
                    await _offerService.ChangeStatusAsync(purchaseOfferModel.OfferId, Domain.OfferStatus.SoldOut, cancellationToken);
                }

                await _userService.DecreaseBalanceAsync(purchaseOfferModel.UserId, totalPrice, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return new PurchaseOfferResponseModel
                {
                    Success = true,
                    Message = "Purchase Done Successfully.",
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                Log.Error(ex, "Unable to make a purchase.", ex.Message);
                return new PurchaseOfferResponseModel
                {
                    Success = false,
                    Message = "Unable to make a purchase."
                };
            }
        }

        public async Task<PurchaseOfferResponseModel> CancelPurchaseAsync(int purchaseId, CancellationToken cancellationToken)
        {
            Purchase? purchase = await _unitOfWork.PurchaseRepository.GetByIdAsync(purchaseId, cancellationToken);
            if (purchase == null) throw new NotFoundException($"Purchase by ID: {purchaseId} not found.");

            if (DateTime.UtcNow > purchase.PurchaseDate.AddMinutes(5)) return new PurchaseOfferResponseModel
            {
                Success = false,
                Message = "Time for refund has expired"
            };

            Offer? offer = await _unitOfWork.OfferRepository.GetByIdAsync(purchase.OfferId, cancellationToken);

            if (offer == null)
            {
                throw new NotFoundException($"Offer by ID: {purchase.OfferId} not found.");
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                await _userService.IncreaseBalanceAsync(purchase.UserId, purchase.TotalPrice, cancellationToken);

                await _offerService.IncreaseAmountAsync(purchase.OfferId, purchase.Quantity, cancellationToken);

                purchase.IsRefunded = true;

                await _unitOfWork.CommitAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return new PurchaseOfferResponseModel
                {
                    Success = true,
                    Message = "Purchase cancelled successfully."
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Could not cancel a purchase by ID: {purchaseId}.", ex.Message);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw new Exception($"Could not cancel a purchase by ID: {purchaseId}.", ex);
            }

        }

        public async Task CancelPurchaseForCancelledOfferAsync(Purchase purchase, CancellationToken cancellationToken)
        {
            try
            {
                if (purchase.IsRefunded) return;
                await _userService.IncreaseBalanceAsync(purchase.UserId, purchase.TotalPrice, cancellationToken);

                await _offerService.IncreaseAmountAsync(purchase.OfferId, purchase.Quantity, cancellationToken);

                _unitOfWork.PurchaseRepository.Delete(purchase);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Could not cancel a purchase by ID: {purchase.Id}.", ex.Message);
                throw new Exception($"Could not cancel a purchase by ID: {purchase.Id}.", ex);
            }
        }
    }
}
