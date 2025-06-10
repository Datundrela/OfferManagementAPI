using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.Authentication.AuthServices.CompanyAuth;
using OfferManagement.Application.EntityServices.Offers.Models;
using OfferManagement.Application.EntityServices.Offers;
using OfferManagement.Common.Extensions;
using OfferManagement.Common.Attributes;

namespace OfferManagement.Web.Controllers
{
    [Route("companyoffers")]
    [Authorize(Roles = "Company")]
    public class CompanyOffersController : Controller
    {
        private readonly IOfferByCompanyService _offerService;
        private readonly ICompanyAuthService _companyAuthService;

        public CompanyOffersController(IOfferByCompanyService offerService, ICompanyAuthService companyAuthService)
        {
            _offerService = offerService;
            _companyAuthService = companyAuthService;
        }

        // GET: /CompanyOffers
        [HttpGet("index")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            int companyId = User.GetIdFromPrincipal();
            var offers = await _offerService.GetCompanyAllOffersAsync(companyId, cancellationToken);
            return View(offers);
        }

        // GET: /CompanyOffers/Add
        [HttpGet("add")]
        [RequireCompanyConfirmation]
        public IActionResult Add()
        {
            return View();
        }

        // POST: /CompanyOffers/Add
        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        [RequireCompanyConfirmation]
        public async Task<IActionResult> Add(IFormCollection form, IFormFile? ImageFile, CancellationToken cancellationToken)
        {

            var model = new AddOfferRequestModel
            {
                Title = form["Title"],
                Description = form["Description"],
                Price = decimal.Parse(form["Price"]),
                Quantity = int.Parse(form["Quantity"]),
                ExpiryDate = DateTime.Parse(form["ExpiryDate"]).ToUniversalTime(),
                CategoryId = int.Parse(form["CategoryId"]),
                Image = ImageFile,
            };

            int companyId = User.GetIdFromPrincipal();
            var response = await _offerService.AddAsync(model, companyId, cancellationToken);

            TempData["Message"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        // POST: /CompanyOffers/Cancel/{offerId}
        [HttpPost("cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int offerId, CancellationToken cancellationToken)
        {
            var response = await _offerService.CancelOfferAsync(offerId, cancellationToken);

            TempData["Message"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

    }

}
