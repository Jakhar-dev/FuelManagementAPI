using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementAPI.Repositories
{
    public class PriceRepository : Repository<Price>, IPriceRepository
    {
        private readonly FuelDbContext _context;

        public PriceRepository(FuelDbContext context) : base(context) 
        {
            _context = context;
        }
       
        public void Add(Price price)
        {
            _context.Prices.Add(price);
        }

        public Price Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Price> GetByIdAsync(int id)
        {
            return await _context.Prices.FirstOrDefaultAsync(p => p.PriceId == id);
        }

        public async Task<Price> GetLatestPriceForProductBeforeDate(int productId, DateTime date)
        {
            return await _context.Prices
                .Where(p => p.ProductId == productId && p.Date != null && p.Date <= date)
                .OrderByDescending(p => p.Date)
                .FirstOrDefaultAsync();
        }

        public Price Update(Price price)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> UpdateProductPricesAsync(PriceUpdateViewModel model)
        {
            try
            {
                var submittedProductIds = model.Products.Select(p => p.ProductId).ToList();

                // Step 1: Remove existing prices for submitted products on the same date
                var existingPrices = await _context.Prices
                    .Where(p => submittedProductIds.Contains(p.ProductId) && p.Date.Date == model.Date.Date)
                    .ToListAsync();

                if (existingPrices.Any())
                {
                    _context.Prices.RemoveRange(existingPrices);
                }

                // Step 2: Save new prices for submitted products
                var submittedPriceEntries = model.Products.Select(p => new Price
                {
                    ProductId = p.ProductId,
                    SellingPrice = p.Price,
                    Date = model.Date
                }).ToList();

                await _context.Prices.AddRangeAsync(submittedPriceEntries);

                // Step 3: Autofill prices for other products in the same category that weren’t submitted
                var allProductIdsInCategory = await _context.Products
                    .Where(p => p.CategoryId == model.CategoryId) // ✅ CORRECTED
                    .Select(p => p.ProductId)
                    .ToListAsync();

                var missingProductIds = allProductIdsInCategory.Except(submittedProductIds).ToList();

                if (missingProductIds.Any())
                {
                    var lastKnownPrices = await _context.Prices
                        .Where(p => missingProductIds.Contains(p.ProductId) && p.Date < model.Date)
                        .GroupBy(p => p.ProductId)
                        .Select(g => g.OrderByDescending(p => p.Date).FirstOrDefault())
                        .ToListAsync();

                    var autoFilledPrices = lastKnownPrices.Select(p => new Price
                    {
                        ProductId = p.ProductId,
                        SellingPrice = p.SellingPrice,
                        Date = model.Date
                    });

                    await _context.Prices.AddRangeAsync(autoFilledPrices);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 ERROR in UpdateProductPricesAsync: " + ex.Message);
                throw; // rethrow to let controller return 500 with log
            }
        }



    }
}
