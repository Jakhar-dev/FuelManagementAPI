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

    public LubeSalesController(ILubeEntryRepository repository)
    {
        _repository = repository;
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
                    Price = s.Price
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
}
