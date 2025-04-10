using FuelManagementAPI.Models.ViewModal;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace FuelManagementAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryRepo.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost("add-category")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool exists = await _categoryRepo.CategoryExistsAsync(model.CategoryName);
            if (exists)
                return Conflict($"Category '{model.CategoryName}' already exists.");

            var addedCategory = await _categoryRepo.AddCategoryAsync(model);
            return Ok(addedCategory);
        }
    }
}
