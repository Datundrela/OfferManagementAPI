using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.EntityServices.Categories;
using OfferManagement.Application.EntityServices.Categories.Models;
using OfferManagement.Application.EntityServices.Companies;
using OfferManagement.Application.Users;
using OfferManagement.Web.Models;

namespace OfferManagement.Web.Controllers
{
    [Route("admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly ICategoryService _categoryService;

        public AdminController(IUserService userService, ICompanyService companyService, ICategoryService categoryService)
        {
            _userService = userService;
            _companyService = companyService;
            _categoryService = categoryService;
        }

        // GET: /Admin
        [HttpGet("index")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllAsync(cancellationToken);
            var companies = await _companyService.GetAllAsync(cancellationToken);
            var categories = await _categoryService.GetAllAsync(cancellationToken);

            var viewModel = new AdminDashboardViewModel
            {
                Users = users,
                Companies = companies,
                Categories = categories
            };

            return View(viewModel);
        }

        // POST: /Admin/ActivateCompany/{companyId}
        [HttpPost("activate-company/{companyId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateCompany(int companyId, CancellationToken cancellationToken)
        {
            await _companyService.ActivateAsync(companyId, cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("deactivate-company/{companyId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateCompany(int companyId, CancellationToken cancellationToken)
        {
            await _companyService.DeactivateAsync(companyId, cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/AddCategory
        [HttpPost("add-category")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(string name, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var categoryDto = new CreateCategoryDTO { Name = name };

            await _categoryService.CreateCategoryAsync(categoryDto, cancellationToken);
            TempData["Message"] = "Category added successfully";

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/DeleteCategory/{categoryId}
        [HttpPost("delete-category/{categoryId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int categoryId, CancellationToken cancellationToken)
        {
            await _categoryService.DeleteByIdAsync(categoryId, cancellationToken);
            TempData["Message"] = "Category deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }

}
