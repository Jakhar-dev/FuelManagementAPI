using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.Security.Claims;

namespace FuelManagementAPI.Repositories
{
    public class PriceRepository : Repository<Price>, IPriceRepository
    {
        private readonly FuelDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PriceRepository(FuelDbContext context, IHttpContextAccessor httpContextAccessor) : base(context) 
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }
       
        public async Task Add(Price price)
        {
            price.UsersId = GetCurrentUserId();
            await _context.Prices.AddAsync(price);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Price>> GetPricesForCurrentUserAsync(int userId)
        {
            return await _context.Prices
                .Include(p => p.Product)
                .Where(p => p.UsersId == userId)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }

        public Price Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Price> GetByIdAsync(int id)
        {
            var userId = GetCurrentUserId();
            return await _context.Prices.FirstOrDefaultAsync(p => p.PriceId == id && p.UsersId == userId);
        }

        public async Task<Price> GetLatestPriceForProductBeforeDate(int productId, DateTime date)
        {
            var userId = GetCurrentUserId();
            return await _context.Prices
                .Where(p => p.ProductId == productId && p.Date != null && p.Date <= date && p.UsersId == userId)
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
                var userId = GetCurrentUserId();
                var submittedProductIds = model.Products.Select(p => p.ProductId).ToList();

                // Step 1: Remove existing prices for submitted products on the same date (user-specific)
                var existingPrices = await _context.Prices
                    .Where(p => submittedProductIds.Contains(p.ProductId) &&
                                p.Date.Date == model.Date.Date &&
                                p.UsersId == userId)
                    .ToListAsync();

                if (existingPrices.Any())
                {
                    _context.Prices.RemoveRange(existingPrices);
                }

                // Step 2: Save new prices for submitted products (assign UsersId)
                var submittedPriceEntries = model.Products.Select(p => new Price
                {
                    ProductId = p.ProductId,
                    SellingPrice = p.Price,
                    Date = model.Date,
                    UsersId = userId
                }).ToList();

                await _context.Prices.AddRangeAsync(submittedPriceEntries);

                // Step 3: Autofill prices for other products in the same category that weren’t submitted
                var allProductIdsInCategory = await _context.Products
                    .Where(p => p.CategoryId == model.CategoryId)
                    .Select(p => p.ProductId)
                    .ToListAsync();

                var missingProductIds = allProductIdsInCategory.Except(submittedProductIds).ToList();

                if (missingProductIds.Any())
                {
                    var lastKnownPrices = await _context.Prices
                        .Where(p => missingProductIds.Contains(p.ProductId) &&
                                    p.Date < model.Date &&
                                    p.UsersId == userId)
                        .GroupBy(p => p.ProductId)
                        .Select(g => g.OrderByDescending(p => p.Date).FirstOrDefault())
                        .ToListAsync();

                    var autoFilledPrices = lastKnownPrices.Select(p => new Price
                    {
                        ProductId = p.ProductId,
                        SellingPrice = p.SellingPrice,
                        Date = model.Date,
                        UsersId = userId
                    });

                    await _context.Prices.AddRangeAsync(autoFilledPrices);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 ERROR in UpdateProductPricesAsync: " + ex.Message);
                throw;
            }
        }       
    }
}
