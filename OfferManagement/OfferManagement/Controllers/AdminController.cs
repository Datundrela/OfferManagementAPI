using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.Users.Models;
using OfferManagement.Application.Users;
using OfferManagement.Application.EntityServices.Admins;
using OfferManagement.Application.EntityServices.Admins.Models;

namespace OfferManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<AdminDTO> Get(int id, CancellationToken cancellationToken)
        {
            return await _adminService.GetByIdAsync(id, cancellationToken);
        }

        [HttpGet]
        public async Task<IEnumerable<AdminDTO>> GetAll(CancellationToken cancellationToken)
        {
            return await _adminService.GetAllAsync(cancellationToken);
        }

        /*[HttpPut]
        public async Task Put(UserPutRequestModel userPutRequestModel, CancellationToken cancellationToken)
        {
            await _userService.UpdateAsync(userPutRequestModel, cancellationToken);
        }*/

        [HttpDelete("{id}")]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await _adminService.DeleteByIdAsync(id, cancellationToken);
        }
    }
}
