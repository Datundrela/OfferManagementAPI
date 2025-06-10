using Microsoft.EntityFrameworkCore;
using OfferManagement.Application.EntityServices.Subscriptions.Models;
using OfferManagement.Application.Exceptions;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;

namespace OfferManagement.Application.EntityServices.Subscriptions
{
    public class SubscriptionByUserService : ISubscriptionByUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionByUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SubscribeToCategoryResponseModel> SubscribeToCategoryAsync(int userId, SubscribeToCategoryRequestModel model, CancellationToken cancellationToken)
        {
            if(!await _unitOfWork.UserRepository.ExistsAsync(x => x.Id == userId, cancellationToken))
            {
                throw new NotFoundException($"User by ID: {userId} not found.");
            }

            if(!await _unitOfWork.CategoryRepository.ExistsAsync(x => x.Id == model.CategoryId, cancellationToken))
            {
                throw new NotFoundException($"Category by ID: {model.CategoryId} not found.");
            }

            if (await _unitOfWork.SubscriptionRepository.GetByUserIdAndCategoryId(userId, model.CategoryId, cancellationToken) != null) return new SubscribeToCategoryResponseModel
            {
                Success = false,
                Message = "Already subscribed"
            };

            Subscription subscription = new Subscription
            {
                UserId = userId,
                CategoryId = model.CategoryId,
            };

            await _unitOfWork.SubscriptionRepository.AddAsync(subscription, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new SubscribeToCategoryResponseModel
            {
                Success = true,
            };
        }

        public async Task<SubscribeToCategoryResponseModel> UnsubscribeFromCategoryAsync(int userId, int categoryId, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.UserRepository.ExistsAsync(x => x.Id == userId, cancellationToken))
            {
                throw new NotFoundException($"User by ID: {userId} not found.");
            }

            if (!await _unitOfWork.CategoryRepository.ExistsAsync(x => x.Id == categoryId, cancellationToken))
            {
                throw new NotFoundException($"Category by ID: {categoryId} not found.");
            }

            var subscription = await _unitOfWork.SubscriptionRepository.GetByUserIdAndCategoryId(userId, categoryId, cancellationToken);
            if( subscription == null ) throw new NotFoundException($"Subscription not found.");

            _unitOfWork.SubscriptionRepository.Delete(subscription);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new SubscribeToCategoryResponseModel
            {
                Success = true,
            };
        }
    }
}
