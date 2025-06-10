using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfferManagement.Application.EntityServices.Categories;
using OfferManagement.Application.EntityServices.Categories.Models;

namespace OfferManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("{id}")]
        public async Task<CategoryDTO> Get(int id, CancellationToken cancellationToken)
        {
            return await _categoryService.GetByIdAsync(id, cancellationToken);
        }

        [HttpGet]
        public async Task<IEnumerable<CategoryDTO>> GetAll(CancellationToken cancellationToken)
        {
            return await _categoryService.GetAllAsync(cancellationToken);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(CreateCategoryDTO categoryDTO, CancellationToken cancellationToken)
        {
            await _categoryService.CreateCategoryAsync(categoryDTO, cancellationToken);

            return Created();
        }

        /*[HttpPut]
        public async Task Put(UserPutRequestModel userPutRequestModel, CancellationToken cancellationToken)
        {
            await _userService.UpdateAsync(userPutRequestModel, cancellationToken);
        }*/

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await _categoryService.DeleteByIdAsync(id, cancellationToken);
        }
    }
}
