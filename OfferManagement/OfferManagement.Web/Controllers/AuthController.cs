using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.Authentication.AuthServices.AdminAuth;
using OfferManagement.Application.Authentication.AuthServices.CompanyAuth;
using OfferManagement.Application.Authentication.AuthServices.Models;
using OfferManagement.Application.Authentication.AuthServices.UserAuth;
using OfferManagement.Application.AuthServices.JWT.Models;
using OfferManagement.Application.EntityServices.Companies.Models;

namespace OfferManagement.Web.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IUserAuthService _userAuthService;
        private readonly ICompanyAuthService _companyAuthService;
        private readonly IAdminAuthService _adminAuthService;

        public AuthController(IUserAuthService userAuthService, ICompanyAuthService companyAuthService, IAdminAuthService adminAuthService)
        {
            _userAuthService = userAuthService;
            _companyAuthService = companyAuthService;
            _adminAuthService = adminAuthService;
        }

        // GET: /Auth/Login
        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userAuthService.LoginAsync(model, cancellationToken);
            if(result.Success)
            {
                StoreTokens(result.Tokens.AccessToken, result.Tokens.RefreshToken);
                return RedirectToAction("Index", "UserCategories");
            } else
            {
                result = await _companyAuthService.LoginAsync(model, cancellationToken);
                if(result.Success)
                {
                    StoreTokens(result.Tokens.AccessToken, result.Tokens.RefreshToken);
                    return RedirectToAction("Index", "CompanyOffers");
                } else
                {
                    result = await _adminAuthService.LoginAsync(model, cancellationToken);
                    if(result.Success)
                    {
                        StoreTokens(result.Tokens.AccessToken, result.Tokens.RefreshToken);
                        return RedirectToAction("Index", "Admin");
                    } else
                    {
                        ModelState.AddModelError("", "Invalid email or password.");
                        return View(model);
                    }
                }
            }
        }

        // GET: /Auth/Register
        [HttpGet("register-user")]
        [AllowAnonymous]
        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpGet("register-company")]
        [AllowAnonymous]
        public IActionResult RegisterCompany()
        {
            return View();
        }

        [HttpGet("register/admin")]
        [AllowAnonymous]
        public IActionResult RegisterAdmin()
        {
            return View();
        }


        // POST: /Auth/Register (User)
        [HttpPost("register-user")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(UserRegisterRequestModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View("Register", model);

            var result = await _userAuthService.RegisterAsync(model, cancellationToken);

            StoreTokens(result.AccessToken, result.RefreshToken);

            return RedirectToAction("Index", "Home");
        }

        // POST: /Auth/Register (Company)
        [HttpPost("register-company")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCompany(CompanyRegisterRequestModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View("Register", model);

            var result = await _companyAuthService.RegisterAsync(model, cancellationToken);

            StoreTokens(result.AccessToken, result.RefreshToken);

            return RedirectToAction("Index", "CompanyOffers");
        }

        // POST: /Auth/Register (Admin)
        [HttpPost("register/admin")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin(UserRegisterRequestModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View("Register", model);

            var result = await _adminAuthService.RegisterAsync(model, cancellationToken);

            StoreTokens(result.AccessToken, result.RefreshToken);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
            {
                return Unauthorized("No refresh token found.");
            }

            if (!Request.Cookies.TryGetValue("AuthToken", out var accessToken))
            {
                return Unauthorized("No refresh token found.");
            }

            AuthTokens tokens = new AuthTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            var newTokens = await _userAuthService.RotateTokensAsync(tokens, cancellationToken);

            if (newTokens == null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            StoreTokens(newTokens.AccessToken, newTokens.RefreshToken);
            return Ok(newTokens);
        }


        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("RefreshToken");
            return RedirectToAction("Login");
        }

        private void StoreTokens(string accessToken, string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("AuthToken", accessToken, cookieOptions);
            Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
        }
    }
}
