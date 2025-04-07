using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

public class FuelSaleRepository : Repository<FuelSale>, IFuelSalesRepository
{
    private readonly FuelDbContext _context;

    public FuelSaleRepository(FuelDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<FuelSale?> GetLastSaleForProductAsync(int productId)
    {
        return await _context.FuelSales
            .Where(s => s.Product.ProductId == productId)
            .OrderByDescending(s => s.FuelSaleId)
            .FirstOrDefaultAsync();
    }

    public async Task AddFuelSaleAsync(FuelSale sale)
    {
        await _context.FuelSales.AddAsync(sale);
    }
}
