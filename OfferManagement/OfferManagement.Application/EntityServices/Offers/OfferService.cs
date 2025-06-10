using Mapster;
using Microsoft.EntityFrameworkCore;
using OfferManagement.Application.EntityServices.Offers.Models;
using OfferManagement.Application.Exceptions;
using OfferManagement.Application.Purchases;
using OfferManagement.Domain;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using Serilog;

namespace OfferManagement.Application.EntityServices.Offers
{
    public class OfferService : IOfferService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OfferService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OfferDTO> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            Offer? offer = await _unitOfWork.OfferRepository.GetByIdAsync(id, cancellationToken);
            if (offer == null) throw new NotFoundException($"Offer by ID:{id} not found.");

            return offer.Adapt<OfferDTO>();
        }

        public async Task<IEnumerable<OfferDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Offer>? offers = await _unitOfWork.OfferRepository.GetAllAsync(cancellationToken);
            if (!offers.Any()) return new List<OfferDTO>();

            return offers.Adapt<IEnumerable<OfferDTO>>();
        }

        public async Task<IEnumerable<OfferDTO>> GetAllByCategoryIdAsync(int categoryId, CancellationToken cancellationToken)
        {
            IEnumerable<Offer>? offers = await _unitOfWork.OfferRepository.GetAllByPredicateQuery(x => x.CategoryId == categoryId).ToListAsync(cancellationToken);
            if (!offers.Any()) return new List<OfferDTO>();

            return offers.Adapt<IEnumerable<OfferDTO>>();
        }

        public async Task<IEnumerable<OfferDTO>> GetAllActiveByCategoryIdAsync(int categoryId, CancellationToken cancellationToken)
        {
            IEnumerable<Offer>? offers = await _unitOfWork.OfferRepository.GetAllActiveByCategoryIdWithImageAsync(categoryId, cancellationToken);
            if (!offers.Any()) return new List<OfferDTO>();

            return offers.Adapt<IEnumerable<OfferDTO>>();
        }

        public async Task CreateAsync(CreateOfferDTO offerDto, CancellationToken cancellationToken)
        {
            await _unitOfWork.OfferRepository.AddAsync(offerDto.Adapt<Offer>(), cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            Offer? offer = await _unitOfWork.OfferRepository.GetByIdAsync(id, cancellationToken);
            if (offer == null) throw new NotFoundException($"Offer by ID:{id} not found.");

            _unitOfWork.OfferRepository.Delete(offer);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task ArchiveExpiredOffersAsync(CancellationToken cancellationToken)
        {
            var expiredOffers = await _unitOfWork.OfferRepository.GetExpiredButHavingActiveStatusOffersAsync(cancellationToken);
            if (expiredOffers.Any())
            {
                foreach (var offer in expiredOffers)
                {
                    offer.OfferStatus = OfferStatus.Expired;
                }
                await _unitOfWork.CommitAsync(cancellationToken);
            }
        }

        public async Task<StockChangeResult> DecreaseAmountAsync(int offerId, int amount, CancellationToken cancellationToken)
        {
            Offer? offer = await _unitOfWork.OfferRepository.GetByIdAsync(offerId, cancellationToken);
            if(offer == null) throw new NotFoundException($"Offer by ID:{offerId} not found.");

            if (offer.Quantity < amount) return new StockChangeResult
            {
                Success = false,
                Message = "Not enough quantity."
            };

            offer.Quantity -= amount;

            if (offer.Quantity == 0)
            {
                offer.OfferStatus = OfferStatus.SoldOut;
            }

            return new StockChangeResult
            {
                Success = true,
                Message = "Stock decreased successfully."
            };
        }

        public async Task<StockChangeResult> IncreaseAmountAsync(int offerId, int amount, CancellationToken cancellationToken)
        {
            Offer? offer = await _unitOfWork.OfferRepository.GetByIdAsync(offerId, cancellationToken);
            if (offer == null) throw new NotFoundException($"Offer by ID:{offerId} not found.");

            offer.Quantity += amount;

            if (offer.OfferStatus == OfferStatus.SoldOut)
            {
                offer.OfferStatus = OfferStatus.Active;
            }

            return new StockChangeResult
            {
                Success = true,
                Message = "Stock increased successfully."
            };
        }

        public async Task<decimal> GetPriceAsync(int offerId, CancellationToken cancellationToken)
        {
            Offer? offer = await _unitOfWork.OfferRepository.GetByIdAsync(offerId, cancellationToken);
            if (offer == null) throw new NotFoundException($"Offer by ID:{offerId} not found.");

            return offer.Price;
        }

        public async Task<int> GetQuantityAsync(int offerId, CancellationToken cancellationToken)
        {
            Offer? offer = await _unitOfWork.OfferRepository.GetByIdAsync(offerId, cancellationToken);
            if (offer == null) throw new NotFoundException($"Offer by ID:{offerId} not found.");

            return offer.Quantity;
        }

        public async Task<OfferStatus> GetStatusAsync(int offerId, CancellationToken cancellationToken)
        {
            Offer? offer = await _unitOfWork.OfferRepository.GetByIdAsync(offerId, cancellationToken);
            if (offer == null) throw new NotFoundException($"Offer by ID:{offerId} not found.");

            return offer.OfferStatus;
        }

        public async Task ChangeStatusAsync(int offerId, OfferStatus newStatus, CancellationToken cancellationToken)
        {
            Offer? offer = await _unitOfWork.OfferRepository.GetByIdAsync(offerId, cancellationToken);
            if (offer == null) throw new NotFoundException($"Offer by ID:{offerId} not found.");

            offer.OfferStatus = newStatus;
        }
    }
}
