using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.EntityServices.Users.Models;
using OfferManagement.Application.Users;
using OfferManagement.Application.Users.Models;

namespace OfferManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<UserDTO> Get(int id, CancellationToken cancellationToken)
        {
            return await _userService.GetByIdAsync(id, cancellationToken);
        }

        [HttpGet]
        public async Task<IEnumerable<UserDTO>> GetAll(CancellationToken cancellationToken)
        {
            return await _userService.GetAllAsync(cancellationToken);
        }

        /*[HttpPut]
        public async Task Put(UserPutRequestModel userPutRequestModel, CancellationToken cancellationToken)
        {
            await _userService.UpdateAsync(userPutRequestModel, cancellationToken);
        }*/

        [HttpDelete("{id}")]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await _userService.DeleteByIdAsync(id, cancellationToken);
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(TransactionRequestModel model, CancellationToken cancellationToken)
        {
            await _userService.IncreaseBalanceWithCommitAsync(model.UserId, model.Amount, cancellationToken);

            return Ok();
        }
    }
}
