using OfferManagement.Application.EntityServices.Subscriptions.Models;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using OfferManagement.Application.Exceptions;
using Mapster;
namespace OfferManagement.Application.EntityServices.Subscriptions
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SubscriptionDTO> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            Subscription? subscription = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id, cancellationToken);
            if (subscription == null) throw new NotFoundException($"Subscription by ID:{id} not found.");

            return subscription.Adapt<SubscriptionDTO>();
        }

        public async Task<IEnumerable<SubscriptionDTO>> GetAllByCategoryIdAsync(int categoryId, CancellationToken cancellationToken)
        {
            IEnumerable<Subscription> subscriptions = await _unitOfWork.SubscriptionRepository.GetAllByCategoryIdAsync(categoryId, cancellationToken);
            if(!subscriptions.Any()) return Enumerable.Empty<SubscriptionDTO>();

            return subscriptions.Adapt<IEnumerable<SubscriptionDTO>>();
        }

        public async Task<IEnumerable<SubscriptionDTO>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            IEnumerable<Subscription> subscriptions = await _unitOfWork.SubscriptionRepository.GetAllByUserIdAsync(userId, cancellationToken);
            if (!subscriptions.Any()) return Enumerable.Empty<SubscriptionDTO>();

            return subscriptions.Adapt<IEnumerable<SubscriptionDTO>>();
        }

        public async Task<IEnumerable<SubscriptionDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Subscription>? subscriptions = await _unitOfWork.SubscriptionRepository.GetAllAsync(cancellationToken);
            if (!subscriptions.Any()) return Enumerable.Empty<SubscriptionDTO>();

            return subscriptions.Adapt<IEnumerable<SubscriptionDTO>>();
        }

        public async Task CreateAsync(CreateSubscriptionDTO subscriptionDto, CancellationToken cancellationToken)
        {
            await _unitOfWork.SubscriptionRepository.AddAsync(subscriptionDto.Adapt<Subscription>(), cancellationToken);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            Subscription? subscription = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id, cancellationToken);
            if (subscription == null) throw new NotFoundException($"Subscription by ID:{id} not found.");

            _unitOfWork.SubscriptionRepository.Delete(subscription);
            await _unitOfWork.CommitAsync();
        }
    }
}
