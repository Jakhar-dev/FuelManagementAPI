using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class FuelSaleRepository : Repository<FuelSale>, IFuelSalesRepository
{
    private readonly FuelDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FuelSaleRepository(FuelDbContext context, IHttpContextAccessor httpContextAccessor)
        : base(context)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var id) ? id : 0;
    }

    public async Task<FuelSale?> GetLastSaleForProductAsync(int productId)
    {
        var userId = GetCurrentUserId();

        return await _context.FuelSales
            .Where(s => s.Product.ProductId == productId && s.UsersId == userId)
            .OrderByDescending(s => s.FuelSaleId)
            .FirstOrDefaultAsync();
    }

    public async Task AddFuelSaleAsync(FuelSale sale)
    {
        sale.UsersId = GetCurrentUserId();
        await _context.FuelSales.AddAsync(sale);
    }

    public async Task DeleteAsync(FuelSale fuelSale)
    {
        _context.FuelSales.Remove(fuelSale);
        await _context.SaveChangesAsync();
    }
}
