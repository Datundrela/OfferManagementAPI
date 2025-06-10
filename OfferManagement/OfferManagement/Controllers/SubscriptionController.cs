using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.EntityServices.Subscriptions;
using OfferManagement.Application.EntityServices.Subscriptions.Models;
using OfferManagement.Common.Extensions;

namespace OfferManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionByUserService _subscriptionByUserService;

        public SubscriptionController(ISubscriptionService subscriptionService, ISubscriptionByUserService subscriptionByUserService)
        {
            _subscriptionService = subscriptionService;
            _subscriptionByUserService = subscriptionByUserService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<SubscriptionDTO> Get(int id, CancellationToken cancellationToken)
        {
            return await _subscriptionService.GetByIdAsync(id, cancellationToken);
        }

        [HttpGet]
        public async Task<IEnumerable<SubscriptionDTO>> GetAll(CancellationToken cancellationToken)
        {
            return await _subscriptionService.GetAllAsync(cancellationToken);
        }

        /*[HttpPut]
        public async Task Put(UserPutRequestModel userPutRequestModel, CancellationToken cancellationToken)
        {
            await _userService.UpdateAsync(userPutRequestModel, cancellationToken);
        }*/

        [HttpDelete("{id}")]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await _subscriptionService.DeleteByIdAsync(id, cancellationToken);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Subscribe(SubscribeToCategoryRequestModel model, CancellationToken cancellationToken)
        {
            int userId = User.GetIdFromPrincipal();

            var result = await _subscriptionByUserService.SubscribeToCategoryAsync(userId, model, cancellationToken);

            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Unsubscribe(int categoryId, CancellationToken cancellationToken)
        {
            int userId = User.GetIdFromPrincipal();

            var result = await _subscriptionByUserService.UnsubscribeFromCategoryAsync(userId, categoryId, cancellationToken);

            if(!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("userSubscriptions")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetSubscriptions(CancellationToken cancellationToken)
        {
            int userId = User.GetIdFromPrincipal();

            var result = await _subscriptionService.GetAllByUserIdAsync(userId, cancellationToken);

            return Ok(result);
        }
    }
}
