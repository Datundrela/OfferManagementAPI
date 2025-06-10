using Microsoft.EntityFrameworkCore;
using OfferManagement.Application.EntityServices.Offers.Models;
using OfferManagement.Application.Exceptions;
using OfferManagement.Domain.Entities;
using OfferManagement.Domain;
using OfferManagement.Infrastructure.UoW;
using Serilog;
using Mapster;
using OfferManagement.Application.EntityServices.Purchases;
using OfferManagement.Application.EntityServices.Images;

namespace OfferManagement.Application.EntityServices.Offers
{
    public class OfferByCompanyService : IOfferByCompanyService
    {
        private readonly IOfferService _offerService;
        private readonly IPurchaseByUserService _purchaseByUserService;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public OfferByCompanyService(IOfferService offerService, IPurchaseByUserService purchaseByUserService, IImageService imageService, IUnitOfWork unitOfWork)
        {
            _offerService = offerService;
            _unitOfWork = unitOfWork;
            _purchaseByUserService = purchaseByUserService;
            _imageService = imageService;
        }

        public async Task<OfferResponseModel> AddAsync(AddOfferRequestModel model, int companhyId, CancellationToken cancellationToken)
        {
            Offer offer = model.Adapt<Offer>();
            offer.CompanyId = companhyId;
            offer.UploadDate = DateTime.UtcNow;
            offer.OfferStatus = OfferStatus.Active;

            if(model.Image != null)
            {
                Image image = await _imageService.UploadAsync(model.Image, cancellationToken);

                offer.ImageId = image.Id;
                offer.Image = image;
            }

            await _unitOfWork.OfferRepository.AddAsync(offer, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new OfferResponseModel
            {
                Success = true,
                Message = "Offer added successfully."
            };
        }

        public async Task<List<OfferDTO>> GetCompanyAllOffersAsync(int companyId, CancellationToken cancellationToken)
        {
            var offers = await _unitOfWork.OfferRepository.GetAllByPredicateQuery(o => o.CompanyId == companyId).Include(o => o.Image).ToListAsync(cancellationToken);
            List<OfferDTO> result = new List<OfferDTO>();

            result.AddRange(offers.Adapt<List<OfferDTO>>());

            return result;
        }

        public async Task<OfferResponseModel> CancelOfferAsync(int offerId, CancellationToken cancellationToken)
        {
            Offer? offer = await _unitOfWork.OfferRepository.GetByIdAsync(offerId, cancellationToken);
            if (offer == null) throw new NotFoundException($"Offer by ID: {offerId} not found.");

            if (DateTime.UtcNow > offer.UploadDate.AddMinutes(10)) return new OfferResponseModel
            {
                Success = false,
                Message = "Offer cancellation period has expired."
            };

            var purchases = await _unitOfWork.PurchaseRepository.GetAllByOfferIdAsync(offerId, cancellationToken);
            if (purchases.Any())
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                if(offer.ImageId != null)
                {
                    await _imageService.DeleteImageAsync((int)offer.ImageId, cancellationToken);
                }

                try
                {
                    foreach (var purchase in purchases)
                    {
                        await _purchaseByUserService.CancelPurchaseForCancelledOfferAsync(purchase, cancellationToken);
                    }

                    _unitOfWork.OfferRepository.Delete(offer);

                    await _unitOfWork.CommitAsync(cancellationToken);
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    return new OfferResponseModel { Success = true, Message = "Offer canceled, and purchases refunded." };
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error while cancellign an offer by ID: {offerId}.", ex.Message);
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    throw new Exception("Failed to cancel offer and refund purchases.", ex);
                }
            }
            else
            {
                if (offer.ImageId != null)
                {
                    await _imageService.DeleteImageAsync((int)offer.ImageId, cancellationToken);
                }

                _unitOfWork.OfferRepository.Delete(offer);
                await _unitOfWork.CommitAsync(cancellationToken);

                return new OfferResponseModel { Success = true, Message = "Offer canceled successfully." };
            }
        }
    }
}
