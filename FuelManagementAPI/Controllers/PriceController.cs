using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace FuelManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController : Controller
    {
        private readonly IPriceRepository _priceRepository;

        public PriceController(IPriceRepository PriceRepository)
        {
            _priceRepository = PriceRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Price>>> GetPrices()
        {
            return Ok(await _priceRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Price>> GetPrice(int id)
        {
            var Price = await _priceRepository.GetByIdAsync(id);
            if (Price == null) return NotFound();
            return Ok(Price);
        }

        [HttpPost]
        public async Task<ActionResult> AddPrice(Price Price)
        {
            await _priceRepository.AddAsync(Price);
            return CreatedAtAction(nameof(GetPrice), new { id = Price.PriceId }, Price);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePrice(int id, Price Price)
        {
            if (id != Price.PriceId) return BadRequest();
            await _priceRepository.UpdateAsync(Price);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePrice(int id)
        {
            await _priceRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("update-prices")]
        public async Task<IActionResult> UpdatePrices([FromBody] PriceUpdateViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Category) || model.Products == null || model.Products.Count == 0)
            {
                return BadRequest("Invalid request data.");
            }

            var result = await _priceRepository.UpdateProductPricesAsync(model);
            if (!result)
            {
                return NotFound("No matching products found to update.");
            }

            return Ok("Prices updated successfully.");
        }

    }
}
