using FuelManagementAPI.Models.ViewModal;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace FuelManagementAPI.Controllers
{
    [Route("api/categoryTypes")]
    [ApiController]
    public class CategoryTypeController : ControllerBase
    {
        private readonly ICategoryTypeRepository _categoryTypeRepo;

        public CategoryTypeController(ICategoryTypeRepository categoryTypeRepo)
        {
            _categoryTypeRepo = categoryTypeRepo;
        }

        [HttpGet("GetCategoryType")]
        public async Task<IActionResult> GetCategoryType()
        {
            var categoryType = await _categoryTypeRepo.GetAllCategoryTypeAsync();
            return Ok(categoryType);
        }

        [HttpGet("by-category/{categoryName}")]
        public async Task<IActionResult> GetByCategory(string categoryName)
        {
            var categoryTypes = await _categoryTypeRepo.GetCategoryTypeByNameAsync(categoryName);
            return Ok(categoryTypes);
        }


        [HttpPost("AddCategoryType")]
        public async Task<IActionResult> AddCategoryType([FromBody] List<CategoryTypeViewModel> models)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var addedCategoryType = await _categoryTypeRepo.AddCategoryTypeAsync(models);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "error occured while adding categories types", details = ex.Message});
            }
        }
    }
}