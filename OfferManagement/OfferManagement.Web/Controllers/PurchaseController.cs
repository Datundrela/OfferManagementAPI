using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.EntityServices.Offers;
using OfferManagement.Application.EntityServices.Purchases;
using OfferManagement.Application.EntityServices.Purchases.Models;
using OfferManagement.Application.Purchases;
using OfferManagement.Common.Extensions;
using OfferManagement.Web.Models;

namespace OfferManagement.Web.Controllers
{
    [Route("purchase")]
    [Authorize(Roles = "User")]
    public class PurchaseController : Controller
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IOfferService _offerService;
        private readonly IPurchaseByUserService _purchaseByUserService;

        public PurchaseController(IPurchaseService purchaseService, IPurchaseByUserService purchaseByUserService, IOfferService offerService)
        {
            _purchaseService = purchaseService;
            _purchaseByUserService = purchaseByUserService;
            _offerService = offerService;
        }

        // GET: /Purchase/Buy/{offerId}
        [HttpGet("buy/{offerId}")]
        public async Task<IActionResult> Buy(int offerId, CancellationToken cancellationToken)
        {
            var offer = await _offerService.GetByIdAsync(offerId, cancellationToken);
            if (offer == null) return NotFound();

            var viewModel = new PurchaseViewModel
            {
                OfferId = offerId,
                OfferTitle = offer.Title,
                Price = offer.Price,
                Quantity = 1
            };

            return View(viewModel);
        }

        // POST: /Purchase/Confirm
        [HttpPost("confirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPurchase(PurchaseViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View("Buy", model);

            int userId = User.GetIdFromPrincipal();
            PurchaseOfferRequestModel requestModel = new PurchaseOfferRequestModel
            {
                OfferId = model.OfferId,
                Quantity = model.Quantity,
                UserId = userId
            };
            var response = await _purchaseByUserService.PurchaseOfferAsync(requestModel, cancellationToken);

            TempData["Message"] = response.Message;
            return RedirectToAction("List");
        }

        // GET: /Purchase/List
        [HttpGet("list")]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            int userId = User.GetIdFromPrincipal();
            var purchases = await _purchaseService.GetAllByUserIdAsync(userId, cancellationToken);
            List<PurchaseMVCDTO> dtos = new List<PurchaseMVCDTO>();
            foreach (var purchase in purchases)
            {
                var offer = await _offerService.GetByIdAsync(purchase.OfferId, cancellationToken);
                var purchaseDto = purchase.Adapt<PurchaseMVCDTO>();
                purchaseDto.OfferTitle = offer.Title;
                dtos.Add(purchaseDto);
            }

            return View(new PurchaseListViewModel { Purchases = dtos });
        }

        // POST: /Purchase/Cancel/{purchaseId}
        [HttpPost("cancel/{purchaseId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelPurchase(int purchaseId, CancellationToken cancellationToken)
        {
            var response = await _purchaseByUserService.CancelPurchaseAsync(purchaseId, cancellationToken);

            TempData["Message"] = response.Message;
            return RedirectToAction("List");
        }
    }

}
