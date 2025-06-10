using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.EntityServices.Admins.Models;
using OfferManagement.Application.EntityServices.Admins;
using OfferManagement.Application.EntityServices.Offers;
using OfferManagement.Application.EntityServices.Offers.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using OfferManagement.Common.Extensions;
using OfferManagement.Common.Attributes;

namespace OfferManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService _offerService;
        private readonly IOfferByCompanyService _offerByCompanyService;

        public OfferController(IOfferService offerService, IOfferByCompanyService offerByCompanyService)
        {
            _offerService = offerService;
            _offerByCompanyService = offerByCompanyService;
        }

        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<OfferDTO> Get(int id, CancellationToken cancellationToken)
        {
            return await _offerService.GetByIdAsync(id, cancellationToken);
        }

        [HttpGet]
        public async Task<IEnumerable<OfferDTO>> GetAll(CancellationToken cancellationToken)
        {
            return await _offerService.GetAllAsync(cancellationToken);
        }

        [HttpPost]
        [Authorize(Roles = "Company")]
        [RequireCompanyConfirmation]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddOffer([FromForm] AddOfferRequestModel model, CancellationToken cancellationToken)
        {
            int companyId = User.GetIdFromPrincipal();
            var result = await _offerByCompanyService.AddAsync(model, companyId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("my-offers")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> GetMyOffers(CancellationToken cancellationToken)
        {

            int companyId = User.GetIdFromPrincipal();
            var offers = await _offerByCompanyService.GetCompanyAllOffersAsync(companyId, cancellationToken);
            return Ok(offers);
        }

        /*[HttpPut]
        public async Task Put(UserPutRequestModel userPutRequestModel, CancellationToken cancellationToken)
        {
            await _userService.UpdateAsync(userPutRequestModel, cancellationToken);
        }*/

        [HttpDelete("cancel/{offerId}")]
        [Authorize(Roles = "Company")]
        [RequireCompanyConfirmation]
        public async Task<IActionResult> CancelOffer(int offerId, CancellationToken cancellationToken)
        {
            int companyId = User.GetIdFromPrincipal();
            var result = await _offerByCompanyService.CancelOfferAsync(offerId, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await _offerService.DeleteByIdAsync(id, cancellationToken);
        }
    }
}
