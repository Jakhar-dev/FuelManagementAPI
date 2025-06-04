using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.Security.Claims;

namespace FuelManagementAPI.Repositories
{
    public class PriceHistoryRepository : Repository<PriceHistory>, IPriceHistoryRepository
    {
        private readonly FuelDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PriceHistoryRepository(FuelDbContext context, IHttpContextAccessor httpContextAccessor) : base(context) 
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }
       
        public async Task Add(PriceHistory price)
        {
            price.UsersId = GetCurrentUserId();
            await _context.PriceHistory.AddAsync(price);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PriceHistory>> GetPricesForCurrentUserAsync(int userId)
        {
            return await _context.PriceHistory
                .Include(p => p.Product)
                .Where(p => p.UsersId == userId)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }

        public PriceHistory Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PriceHistory> GetByIdAsync(int id)
        {
            var userId = GetCurrentUserId();
            return await _context.PriceHistory.FirstOrDefaultAsync(p => p.PriceId == id && p.UsersId == userId);
        }

        public async Task<PriceHistory> GetPriceForProductByDate(int productId, DateTime date)
        {
            var userId = GetCurrentUserId();

            var startDate = date.Date;
            var endDate = startDate.AddDays(1);
            return await _context.PriceHistory
                .Where(p =>
                p.ProductId == productId && 
                p.Date >= startDate &&
                p.Date < endDate &&
                p.UsersId == userId)
                .FirstOrDefaultAsync();
        }

        public PriceHistory Update(PriceHistory price)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> UpdateProductPricesAsync(PriceUpdateViewModel model)
        {
            try
            {
                var userId = GetCurrentUserId();
                var submittedProductIds = model.Products.Select(p => p.ProductId).ToList();

                // Step 1: Remove existing prices for submitted products on the same date, user, and price type
                var existingPrices = await _context.PriceHistory
                    .Where(p =>
                        submittedProductIds.Contains(p.ProductId) &&
                        p.Date.Date == model.Date.Date &&
                        p.UsersId == userId &&
                        p.PriceType == model.PriceType &&
                        p.CategoryTypeId == model.CategoryTypeId)
                    .ToListAsync();

                if (existingPrices.Any())
                {
                    _context.PriceHistory.RemoveRange(existingPrices);
                }

                // Step 2: Save new prices
                var submittedPriceEntries = model.Products.Select(p => new PriceHistory
                {
                    ProductId = p.ProductId,
                    Price = p.Price,
                    Date = model.Date,
                    UsersId = userId,
                    CategoryId = model.CategoryId,
                    CategoryTypeId = model.CategoryTypeId,
                    PriceType = model.PriceType
                }).ToList();

                await _context.PriceHistory.AddRangeAsync(submittedPriceEntries);

                // Step 3: Auto-fill missing product prices from same category type
                var allProductIdsInCategoryType = await _context.Products
                    .Where(p => p.CategoryTypeId == model.CategoryTypeId)
                    .Select(p => p.ProductId)
                    .ToListAsync();

                var missingProductIds = allProductIdsInCategoryType.Except(submittedProductIds).ToList();

                if (missingProductIds.Any())
                {
                    var lastKnownPrices = await _context.PriceHistory
                        .Where(p =>
                            missingProductIds.Contains(p.ProductId) &&
                            p.Date < model.Date &&
                            p.UsersId == userId &&
                            p.PriceType == model.PriceType &&
                            p.CategoryTypeId == model.CategoryTypeId)
                        .GroupBy(p => p.ProductId)
                        .Select(g => g.OrderByDescending(p => p.Date).FirstOrDefault())
                        .ToListAsync();

                    var autoFilledPrices = lastKnownPrices.Select(p => new PriceHistory
                    {
                        ProductId = p.ProductId,
                        Price = p.Price,
                        Date = model.Date,
                        UsersId = userId,
                        CategoryId = p.CategoryId,
                        CategoryTypeId = p.CategoryTypeId,
                        PriceType = model.PriceType
                    });

                    await _context.PriceHistory.AddRangeAsync(autoFilledPrices);
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
