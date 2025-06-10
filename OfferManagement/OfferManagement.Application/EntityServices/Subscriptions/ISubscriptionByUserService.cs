using OfferManagement.Application.EntityServices.Subscriptions.Models;

namespace OfferManagement.Application.EntityServices.Subscriptions
{
    public interface ISubscriptionByUserService
    {
        Task<SubscribeToCategoryResponseModel> SubscribeToCategoryAsync(int userId, SubscribeToCategoryRequestModel model, CancellationToken cancellationToken);
        Task<SubscribeToCategoryResponseModel> UnsubscribeFromCategoryAsync(int userId, int subscriptionId, CancellationToken cancellationToken);
    }
}
