using FuelManagementAPI.Models.ViewModal;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using FuelManagementAPI.Data;

namespace FuelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/Purchase")]
    public class PurchaseController : ControllerBase
    {
        private readonly IFuelPurchaseRepository _fuelRepo;
        private readonly ILubePurchaseRepository _lubeRepo;
        private readonly FuelDbContext _context;

        public PurchaseController(
            IFuelPurchaseRepository fuelRepo,
            ILubePurchaseRepository lubeRepo,
            FuelDbContext context)
        {
            _fuelRepo = fuelRepo;
            _lubeRepo = lubeRepo;
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddPurchase([FromBody] PurchaseEntryViewModel dto)
        {
            if (dto == null || dto.PurchaseDate == default)
            {
                return BadRequest("Invalid purchase data.");
            }
            try
            {
                if (dto.FuelPurchases != null)
                {
                    foreach (var fuel in dto.FuelPurchases)
                    {
                        var fuelEntity = new FuelPurchase
                        {
                            PurchaseDate = dto.PurchaseDate,
                            CategoryTypeId = fuel.ProductCategoryTypeId,
                            PurchaseQuantity = fuel.PurchaseQuantity,
                            PurchasePrice = fuel.PurchasePrice,
                            Amount = fuel.Amount,
                            Description = fuel.Description
                        };
                        await _fuelRepo.AddAsync(fuelEntity);
                     //   await _fuelRepo.UpdateCategoryPriceAsync(fuel.ProductCategoryTypeId, fuel.PurchasePrice);
                    }
                }

                if (dto.LubePurchases != null)
                {
                    foreach (var lube in dto.LubePurchases)
                    {
                        var lubeEntity = new LubePurchase
                        {
                            PurchaseDate = dto.PurchaseDate,
                            ProductCategoryTypeId = lube.ProductCategoryTypeId,
                            ProductId = lube.ProductId,
                            PurchaseQuantity = lube.PurchaseQuantity,
                            PurchasePrice = lube.PurchasePrice,
                            Amount = lube.Amount,
                            Description = lube.Description
                        };

                        await _lubeRepo.AddAsync(lubeEntity);
                      //  await _lubeRepo.UpdateProductPriceAsync(lube.ProductId, lube.PurchasePrice);
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Purchase saved successfully." });
            }
            catch (Exception ex)
            {
                // Ideally log the error
                return StatusCode(500, "An error occurred while saving the purchase.");
            }
        }
    }
}
