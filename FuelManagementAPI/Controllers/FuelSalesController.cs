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
        private readonly IFuelEntryRepository _FuelEntryRepository;
        private readonly IFuelSalesRepository _FuelSalesRepository;

        public FuelSalesController(IFuelEntryRepository fuelEntryRepository, IFuelSalesRepository fuelSalesRepository)
        {
            _FuelEntryRepository = fuelEntryRepository;
            _FuelSalesRepository = fuelSalesRepository;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetFuelSales()
        {
            return Ok(await _FuelEntryRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FuelSale>> GetFuelSales(int id)
        {
            var FuelSales = await _FuelEntryRepository.GetByIdAsync(id);
            if (FuelSales == null) return NotFound();
            return Ok(FuelSales);
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
                        Price = s.Price
                    }).ToList()
                };
           
            var addedEntry = await _FuelEntryRepository.AddAsync(fuelEntry);
            return Ok(addedEntry);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFuelSales(int id, FuelEntry FuelEntry)
        {
            if (id != FuelEntry.FuelEntryId) return BadRequest();
            await _FuelEntryRepository.UpdateAsync(FuelEntry);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFuelSales(int id)
        {
            await _FuelEntryRepository.DeleteAsync(id);
            return NoContent();
        }

        // FuelSalesController.cs
        //[HttpGet("previous-reading")]
        //public async Task<IActionResult> GetPreviousReading(
        // [FromQuery] int productId,
        // [FromQuery] DateTime entryDate)
        //{
        //    var previousReading = await _FuelEntryRepository.GetPreviousReadingAsync(productId, entryDate);
        //    return Ok(previousReading);
        //}

        [HttpGet("previous-reading")]
        public async Task<IActionResult> GetPreviousReading([FromQuery] int productId)
        {
            var lastSale = await _FuelSalesRepository.GetLastSaleForProductAsync(productId);
            return Ok(new { previousReading = lastSale?.CurrentReading ?? 0 });
        }

        [HttpDelete("fuel/{id}")]
        public async Task<IActionResult> DeleteFuelSale(int id)
        {
            var sale = await _FuelSalesRepository.GetByIdAsync(id);
            if (sale == null)
                return NotFound();

            await _FuelSalesRepository.DeleteAsync(sale);
            return NoContent();
        }

    }
}
