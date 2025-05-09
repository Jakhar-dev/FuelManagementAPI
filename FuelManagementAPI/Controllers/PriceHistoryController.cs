using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace FuelManagementAPI.Controllers
{
    [Route("api/price")]
    [ApiController]
    public class PriceHistoryController : Controller
    {
        private readonly IPriceHistoryRepository _priceRepository;

        public PriceHistoryController(IPriceHistoryRepository PriceRepository)
        {
            _priceRepository = PriceRepository;
        }

        private int GetCurrentUserId()
        {
            return int.TryParse(User?.FindFirst("id")?.Value, out var id) ? id : 0;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceHistory>>> GetPrices()
        {
            return Ok(await _priceRepository.GetAllAsync());
        }

        [HttpGet("user-prices")]
        public async Task<IActionResult> GetUserPrices()
        {
            try
            {
                var userId = GetCurrentUserId(); // Assumes you have this helper method
                var prices = await _priceRepository.GetPricesForCurrentUserAsync(userId);

                return Ok(prices);
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 Error fetching user prices: " + ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<PriceHistory>> GetPrice(int id)
        {
            var Price = await _priceRepository.GetByIdAsync(id);
            if (Price == null) return NotFound();
            return Ok(new {sellingPrice = Price.Price});
        }

        [HttpGet("price-by-product-and-date")]
        public async Task<IActionResult> GetPriceByProductAndDate([FromQuery] int productId, [FromQuery] DateTime date)
        {
            try
            {
                var price = await _priceRepository.GetLatestPriceForProductBeforeDate(productId, date);

                if (price == null)
                    return Ok(new { sellingPrice = 0 }); // Instead of NotFound

                return Ok(new { Price = price.Price });
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 Error fetching price: " + ex.Message);
                return StatusCode(500, "Error fetching price: " + ex.Message);
            }
        }


        [HttpPost]
        public async Task<ActionResult> AddPrice(PriceHistory Price)
        {
            await _priceRepository.AddAsync(Price);
            return CreatedAtAction(nameof(GetPrice), new { id = Price.PriceId }, Price);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePrice(int id, PriceHistory Price)
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
            if (model == null || model.CategoryId == 0 || model.Products == null || model.Products.Count == 0)
                return BadRequest("Invalid request data.");

            model.Date = DateTime.SpecifyKind(model.Date, DateTimeKind.Utc);

            try
            {
                var result = await _priceRepository.UpdateProductPricesAsync(model);

                if (!result)
                    return NotFound("No matching products found to update.");

                return Ok("Prices updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 CONTROLLER ERROR: " + ex.Message);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}
