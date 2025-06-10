using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.EntityServices.Categories;
using OfferManagement.Application.EntityServices.Offers;
using OfferManagement.Application.EntityServices.Offers.Models;
using OfferManagement.Application.EntityServices.Subscriptions;
using OfferManagement.Application.Users;
using OfferManagement.Application.Users.Models;
using OfferManagement.Common.Extensions;
using OfferManagement.Domain.Entities;
using OfferManagement.Web.Models;

namespace OfferManagement.Web.Controllers
{
    [Route("usercategories")]
    [Authorize(Roles = "User")]
    public class UserCategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionByUserService _subscriptionByUserService;
        private readonly IOfferService _offerService;
        private readonly IUserService _userService;

        public UserCategoriesController(
            ICategoryService categoryService, 
            ISubscriptionService subscriptionService, 
            ISubscriptionByUserService subscriptionByUserService, 
            IOfferService offerService,
            IUserService userService)
        {
            _categoryService = categoryService;
            _subscriptionService = subscriptionService;
            _subscriptionByUserService = subscriptionByUserService;
            _offerService = offerService;
            _userService = userService;
        }

        [HttpGet("index")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            int userId = User.GetIdFromPrincipal();
            UserDTO user = await _userService.GetByIdAsync(userId, cancellationToken);
            var categories = await _categoryService.GetAllAsync(cancellationToken);
            var subscribedCategories = await _subscriptionService.GetAllByUserIdAsync(userId, cancellationToken);
            var subscribedCategoryIds = subscribedCategories.Select(c => c.CategoryId).ToList();
            var activeOffers = new List<OfferDTO>();
            foreach(var subscribedCategoryId in subscribedCategoryIds)
            {
                activeOffers.AddRange(await _offerService.GetAllActiveByCategoryIdAsync(subscribedCategoryId, cancellationToken));
            }

            var viewModel = new UserCategoriesViewModel
            {
                Categories = categories,
                SubscribedCategoryIds = subscribedCategoryIds,
                Offers = activeOffers,
                Balance = user.Balance,

            };

            return View(viewModel);
        }

        [HttpPost("subscribe/{categoryId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(int categoryId, CancellationToken cancellationToken)
        {
            int userId = User.GetIdFromPrincipal();
            var model = new Application.EntityServices.Subscriptions.Models.SubscribeToCategoryRequestModel { CategoryId = categoryId };
            await _subscriptionByUserService.SubscribeToCategoryAsync(userId, model, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("unsubscribe/{categoryId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unsubscribe(int categoryId, CancellationToken cancellationToken)
        {
            int userId = User.GetIdFromPrincipal();
            await _subscriptionByUserService.UnsubscribeFromCategoryAsync(userId, categoryId, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("deposit")]
        public async Task<IActionResult> Deposit(CancellationToken cancellationToken)
        {
            return View();
        }

        [HttpPost("deposit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(decimal amount, CancellationToken cancellationToken)
        {
            int userId = User.GetIdFromPrincipal();
            var response = await _userService.DepositAsync(userId, amount, cancellationToken);

            TempData["Message"] = response.Message;

            if (response.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Deposit));
            }
        }
    }

}
