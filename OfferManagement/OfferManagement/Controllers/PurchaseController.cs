using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.EntityServices.Offers.Models;
using OfferManagement.Application.EntityServices.Offers;
using OfferManagement.Application.Purchases;
using OfferManagement.Application.Purchases.Models;
using OfferManagement.Application.EntityServices.Purchases.Models;
using OfferManagement.Application.EntityServices.Purchases;

namespace OfferManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IPurchaseByUserService _purchaseByUserService;

        public PurchaseController(IPurchaseService purchaseService, IPurchaseByUserService purchaseByUserService)
        {
            _purchaseService = purchaseService;
            _purchaseByUserService = purchaseByUserService;
        }

        [HttpGet("{id}")]
        public async Task<PurchaseDTO> Get(int id, CancellationToken cancellationToken)
        {
            return await _purchaseService.GetByIdAsync(id, cancellationToken);
        }

        [HttpGet]
        public async Task<IEnumerable<PurchaseDTO>> GetAll(CancellationToken cancellationToken)
        {
            return await _purchaseService.GetAllAsync(cancellationToken);
        }

        /*[HttpPut]
        public async Task Put(UserPutRequestModel userPutRequestModel, CancellationToken cancellationToken)
        {
            await _userService.UpdateAsync(userPutRequestModel, cancellationToken);
        }*/

        [HttpDelete("{id}")]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await _purchaseService.DeleteByIdAsync(id, cancellationToken);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PurchaseOffer([FromBody] PurchaseOfferRequestModel request, CancellationToken cancellationToken)
        {
            var response = await _purchaseByUserService.PurchaseOfferAsync(request, cancellationToken);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("cancel/{purchaseId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CancelPurchase(int purchaseId, CancellationToken cancellationToken)
        {
            var response = await _purchaseByUserService.CancelPurchaseAsync(purchaseId, cancellationToken);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
