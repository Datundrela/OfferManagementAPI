using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.EntityServices.Companies;
using OfferManagement.Application.EntityServices.Companies.Models;

namespace OfferManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<CompanyDTO> Get(int id, CancellationToken cancellationToken)
        {
            return await _companyService.GetByIdAsync(id, cancellationToken);
        }

        [HttpGet]
        public async Task<IEnumerable<CompanyDTO>> GetAll(CancellationToken cancellationToken)
        {
            return await _companyService.GetAllAsync(cancellationToken);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateCompanyDTO companyDTO, CancellationToken cancellationToken)
        {
            await _companyService.CreateAsync(companyDTO, cancellationToken);

            return Created();
        }

        /*[HttpPut]
        public async Task Put(UserPutRequestModel userPutRequestModel, CancellationToken cancellationToken)
        {
            await _companyService.UpdateAsync(userPutRequestModel, cancellationToken);
        }*/

        [HttpDelete("{id}")]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await _companyService.DeleteByIdAsync(id, cancellationToken);
        }

        [HttpPut("activate/{companyId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Activate(int companyId, CancellationToken cancellationToken)
        {
            await _companyService.ActivateAsync(companyId, cancellationToken);

            return Ok();
        }

        [HttpPut("deactivate/{companyId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(int companyId, CancellationToken cancellationToken)
        {
            await _companyService.DeactivateAsync(companyId, cancellationToken);

            return Ok();
        }
    }
}
