using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.Authentication.AuthServices.AdminAuth;
using OfferManagement.Application.Authentication.AuthServices.CompanyAuth;
using OfferManagement.Application.Authentication.AuthServices.Models;
using OfferManagement.Application.Authentication.AuthServices.UserAuth;
using OfferManagement.Application.AuthServices.JWT.Models;
using System.Net;

namespace OfferManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserAuthService _userAuthService;
        private readonly ICompanyAuthService _companyAuthService;
        private readonly IAdminAuthService _adminAuthService;

        public AuthController(
            IUserAuthService userAuthService,
            ICompanyAuthService companyAuthService,
            IAdminAuthService adminAuthService)
        {
            _userAuthService = userAuthService;
            _companyAuthService = companyAuthService;
            _adminAuthService = adminAuthService;
        }

        [HttpPost("user/login")]
        public async Task<IActionResult> UserLogin([FromBody] LoginRequestModel model, CancellationToken cancellationToken)
        {
            var result = await _userAuthService.LoginAsync(model, cancellationToken);
            if (!result.Success) return Unauthorized("Incorrect email or password");

            return Ok(result.Tokens);
        }

        [HttpPost("user/register")]
        public async Task<IActionResult> UserRegister([FromBody] UserRegisterRequestModel model, CancellationToken cancellationToken)
        {
            var tokens = await _userAuthService.RegisterAsync(model, cancellationToken);

            return Ok(tokens);
        }

        [HttpPost("user/rotateTokens")]
        public async Task<IActionResult> UserRotateTokens([FromBody] AuthTokens tokens, CancellationToken cancellationToken)
        {
            var newTokens = await _userAuthService.RotateTokensAsync(tokens, cancellationToken);

            return Ok(newTokens);
        }

        [HttpPost("company/login")]
        public async Task<IActionResult> CompanyLogin([FromBody] LoginRequestModel model, CancellationToken cancellationToken)
        {
            var result = await _companyAuthService.LoginAsync(model, cancellationToken);
            if (!result.Success) return Unauthorized("Incorrect email or password");

            return Ok(result.Tokens);
        }

        [HttpPost("company/register")]
        public async Task<IActionResult> CompanyRegister(CompanyRegisterRequestModel model, CancellationToken cancellationToken)
        {
            var tokens = await _companyAuthService.RegisterAsync(model, cancellationToken);

            return Ok(tokens);
        }

        [HttpPost("company/rotateTokens")]
        public async Task<IActionResult> CompanyRotateTokens([FromBody] AuthTokens tokens, CancellationToken cancellationToken)
        {
            var newTokens = await _companyAuthService.RotateTokensAsync(tokens, cancellationToken);

            return Ok(newTokens);
        }

        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequestModel model, CancellationToken cancellationToken)
        {
            var result = await _adminAuthService.LoginAsync(model, cancellationToken);
            if (!result.Success) return Unauthorized("Incorrect email or password");

            return Ok(result.Tokens);
        }

        [HttpPost("admin/register")]
        public async Task<IActionResult> AdminRegister(UserRegisterRequestModel model, CancellationToken cancellationToken)
        {
            var tokens = await _adminAuthService.RegisterAsync(model, cancellationToken);

            return Ok(tokens);
        }

        [HttpPost("admin/rotateTokens")]
        public async Task<IActionResult> AdminRotateTokens([FromBody] AuthTokens tokens, CancellationToken cancellationToken)
        {
            var newTokens = await _adminAuthService.RotateTokensAsync(tokens, cancellationToken);

            return Ok(newTokens);
        }
    }
}
