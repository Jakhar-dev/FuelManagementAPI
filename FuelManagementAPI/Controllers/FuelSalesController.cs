using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementAPI.Controllers
{
    [Route("api/fuelSales")]
    [ApiController]
    [EnableCors("AllowLocalhost")]
    public class FuelSalesController : Controller
    {
        private readonly IFuelEntryRepository _fuelEntryRepository;
        private readonly IFuelSalesRepository _fuelSalesRepository;

        public FuelSalesController(IFuelEntryRepository fuelEntryRepository, IFuelSalesRepository fuelSalesRepository)
        {
            _fuelEntryRepository = fuelEntryRepository;
            _fuelSalesRepository = fuelSalesRepository;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetFuelSales()
        {
            try
            {
                var fuelSales = await _fuelEntryRepository.GetAllAsync();
                return Ok(fuelSales);
            }
            catch (Exception ex)
            {
                // Log it properly in production
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetFuelSaleById(int id)
        {
            var sale = await _fuelEntryRepository.GetByIdAsync(id);
            if (sale == null)
                return NotFound();

            return Ok(sale);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddFuelSales([FromBody] FuelEntryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var fuelEntry = new FuelEntry
                {
                    Date = model.Date,
                    Sales = model.Sales.Select(s => new FuelSale
                    {
                        ProductId = s.ProductId,
                        PreviousReading = s.PreviousReading,
                        CurrentReading = s.CurrentReading,
                        Testing = s.Testing,
                        SaleQuantity = (s.CurrentReading - s.PreviousReading - s.Testing),
                        Price = s.Price,
                        Amount = (s.CurrentReading - s.PreviousReading - s.Testing) * s.Price
                    }).ToList()
                };

                var addedEntry = await _fuelEntryRepository.AddAsync(fuelEntry);
                return Ok(addedEntry);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFuelSales(int id, [FromBody] FuelEntry fuelEntry)
        {
            if (id != fuelEntry.FuelEntryId)
                return BadRequest("ID mismatch.");

            await _fuelEntryRepository.UpdateAsync(fuelEntry);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFuelSales(int id)
        {
            await _fuelEntryRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("check-duplicate")]
        public async Task<IActionResult> CheckDuplicateSale([FromQuery] int productId, [FromQuery] DateTime date)
        {
            var exists = await _fuelEntryRepository.SaleExistsAsync(productId, date);
            return Ok(new { exists });
        }

        [HttpGet("previous-reading")]
        public async Task<IActionResult> GetPreviousReading([FromQuery] int productId)
        {
            var lastSale = await _fuelSalesRepository.GetLastSaleForProductAsync(productId);
            var previousReading = lastSale?.CurrentReading ?? 0;
            return Ok(new { previousReading });
        }

        [HttpDelete("fuel/{id}")]
        public async Task<IActionResult> DeleteSingleFuelSale(int id)
        {
            var sale = await _fuelSalesRepository.GetByIdAsync(id);
            if (sale == null)
                return NotFound();

            await _fuelSalesRepository.DeleteAsync(sale);
            return NoContent();
        }
    }
}
