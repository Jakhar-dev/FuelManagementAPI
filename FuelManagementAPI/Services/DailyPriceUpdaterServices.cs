using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementAPI.Services
{
    public class DailyPriceUpdaterService
    {
        private readonly FuelDbContext _context;

        public DailyPriceUpdaterService(FuelDbContext context)
        {
            _context = context;
        }

        public async Task UpdateDailyPricesAsync()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var allProductIds = await _context.Products.Select(p => p.ProductId).ToListAsync();
            var productsWithTodayPrice = await _context.Prices
                .Where(p => p.Date == today)
                .Select(p => p.ProductId)
                .ToListAsync();

            var missingProducts = allProductIds.Except(productsWithTodayPrice).ToList();

            if (!missingProducts.Any()) return;

            var yesterdaysPrices = await _context.Prices
                .Where(p => missingProducts.Contains(p.ProductId) && p.Date == yesterday)
                .ToListAsync();

            if (!yesterdaysPrices.Any()) return;

            var todayPrices = yesterdaysPrices.Select(p => new Price
            {
                ProductId = p.ProductId,
                SellingPrice = p.SellingPrice,
                Date = today,
                Description = "Auto-copied from yesterday"
            }).ToList();

            await _context.Prices.AddRangeAsync(todayPrices);
            await _context.SaveChangesAsync();
        }
    }
}
