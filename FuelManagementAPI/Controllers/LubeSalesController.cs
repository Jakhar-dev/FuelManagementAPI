using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[Route("api/LubeSales")]
[ApiController]
public class LubeSalesController : ControllerBase
{
    private readonly ILubeEntryRepository _repository;
    private readonly ILubeSalesRepository _salesRepository;

    public LubeSalesController(ILubeEntryRepository repository, ILubeSalesRepository salesRepository)
    {
        _repository = repository;
        _salesRepository = salesRepository;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddLubeSales([FromBody] LubeEntryViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var lubeEntry = new LubeEntry
            {
                Date = model.Date,
                Sales = model.Sales.Select(s => new LubeSale
                {
                    ProductId = s.ProductId,
                    Quantity = s.Quantity,
                    Price = s.Price,
                    Amount = s.Quantity * s.Price
                }).ToList()
            };

            var addedEntry = await _repository.AddAsync(lubeEntry);
            return Ok(addedEntry);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.ToString());
        }
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllLubeSales()
    {
        var entries = await _repository.GetAllAsync();
        return Ok(entries);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLubeSale(int id)
    {
        try
        {
            var sale = await _salesRepository.GetByIdAsync(id);
            if (sale == null)
                return NotFound();

            await _salesRepository.DeleteAsync(sale);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting lube sale: {ex.Message}");
        }
    }


}
