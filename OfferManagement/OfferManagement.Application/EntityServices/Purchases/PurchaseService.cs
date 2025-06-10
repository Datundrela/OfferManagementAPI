using Mapster;
using Microsoft.EntityFrameworkCore;
using OfferManagement.Application.EntityServices.Offers;
using OfferManagement.Application.EntityServices.Offers.Models;
using OfferManagement.Application.EntityServices.Purchases.Models;
using OfferManagement.Application.EntityServices.Users.Models;
using OfferManagement.Application.Exceptions;
using OfferManagement.Application.Purchases;
using OfferManagement.Application.Purchases.Models;
using OfferManagement.Application.Users;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using Serilog;
using System;

namespace OfferManagement.Application.EntityServices.Purchases
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IOfferService _offerService;

        public PurchaseService(IUnitOfWork unitOfWork, IUserService userService, IOfferService offerService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _offerService = offerService;
        }

        public async Task<PurchaseDTO> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            Purchase? purchase = await _unitOfWork.PurchaseRepository.GetByIdAsync(id, cancellationToken);
            if (purchase == null) throw new NotFoundException($"Purchase by ID:{id} not found.");

            return purchase.Adapt<PurchaseDTO>();
        }

        public async Task<IEnumerable<PurchaseDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Purchase>? purchases = await _unitOfWork.PurchaseRepository.GetAllAsync(cancellationToken);
            if (!purchases.Any()) return new List<PurchaseDTO>();

            return purchases.Adapt<IEnumerable<PurchaseDTO>>();
        }

        public async Task<IEnumerable<PurchaseDTO>> GetAllByOfferIdAsync(int offerId, CancellationToken cancellationToken)
        {
            IEnumerable<Purchase> purchases = await _unitOfWork.PurchaseRepository.GetAllByPredicateQuery(x => x.OfferId == offerId).ToListAsync(cancellationToken);
            if (!purchases.Any()) return new List<PurchaseDTO>();

            return purchases.Adapt<IEnumerable<PurchaseDTO>>();
        }

        public async Task<IEnumerable<PurchaseDTO>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            IEnumerable<Purchase> purchases = await _unitOfWork.PurchaseRepository.GetAllByPredicateQuery(x => x.UserId == userId).ToListAsync(cancellationToken);
            if (!purchases.Any()) return new List<PurchaseDTO>();

            return purchases.Adapt<IEnumerable<PurchaseDTO>>();
        }

        public async Task CreateAsync(CreatePurchaseDTO purchaseDto, CancellationToken cancellationToken)
        {
            await _unitOfWork.PurchaseRepository.AddAsync(purchaseDto.Adapt<Purchase>(), cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            Purchase? purchase = await _unitOfWork.PurchaseRepository.GetByIdAsync(id, cancellationToken);
            if (purchase == null) throw new NotFoundException($"Purchase by ID:{id} not found.");

            _unitOfWork.PurchaseRepository.Delete(purchase);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
