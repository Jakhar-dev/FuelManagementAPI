using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FuelManagementAPI.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly FuelDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public ProductRepository(FuelDbContext context, IHttpContextAccessor contextAccessor) : base(context)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }
        public async Task AddProductAsync(Product product)
        {
            product.UsersId = GetCurrentUserId();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        //public async Task AddProductsAsync(IEnumerable<Product> products)
        //{
        //    await _context.Products.AddRangeAsync(products);
        //    await _context.SaveChangesAsync();
        //}

        public Task<Product> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetByIdAsync(int id)
        {
            return await _context.Products
                .Where(s => s.ProductId == id)
                .ToListAsync();
        }
       
        public Task<List<Product>> GetProductsAsync()
        {
            var userId = GetCurrentUserId();
            return _context.Products
                .Where(c => c.UsersId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryNameAsync(string categoryTypeName)
        {
            var userId = GetCurrentUserId();
            return await _context.Products
                .Include(p => p.ProductCategoryType)
                .Where(p => p.UsersId == userId && p.ProductCategoryType.CategoryTypeName.ToLower() == categoryTypeName.ToLower())
                .ToListAsync();
        }
        public async Task<ProductCategoryType> GetCategoryByIdAsync(int id)
        {
            return await _context.ProductCategoriesType.FindAsync(id);
        }
        public async Task<IEnumerable<Product>> GetProductsByCategoryTypeIdAsync(int categoryTypeId)
        {
            var userId = GetCurrentUserId();
            return await _context.Products
                .Include(p => p.ProductCategoryType)
                .Where(p => p.UsersId == userId && p.CategoryTypeId == categoryTypeId)
                .ToListAsync();
        }


        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var userId = GetCurrentUserId();
            return await _context.Products
                .Include(p => p.ProductCategoryType)
                .Where(p => p.CategoryTypeId == categoryId && p.UsersId == userId)
                .ToListAsync();
        }
        public async Task<ProductCategory?> GetCategoryByNameAsync(string categoryName)
        {
            var userId = GetCurrentUserId();
            return await _context.ProductCategories
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryName.ToLower() && c.UsersId == userId);
        }
        public async Task<bool> ProductExistsAsync(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return false;

            var userId = GetCurrentUserId();

            return await _context.Products
                .AnyAsync(p => p.ProductName.ToLower() == productName.Trim().ToLower() && p.UsersId == userId);
        }

        public async Task<List<SalesChartViewModel>> GetSalesChartByProductAsync(string range, int? categoryId, int? productId)
        {
            var userId = GetCurrentUserId();
            DateTime fromDate = range switch
            {
                "day" => DateTime.UtcNow.Date.AddDays(-7),
                "month" => DateTime.UtcNow.Date.AddMonths(-6),
                "quarter" => DateTime.UtcNow.Date.AddMonths(-12),
                "year" => DateTime.UtcNow.Date.AddYears(-3),
                _ => DateTime.UtcNow.Date.AddDays(-7),
            };

            var fuelSalesQuery = _context.FuelSales
                .Where(f => f.UsersId == userId && f.FuelEntry.Date >= fromDate)
                .Where(f => !productId.HasValue || f.ProductId == productId)
                .Where(f => !categoryId.HasValue || f.Product.ProductCategoryType.CategoryId == categoryId)
                .Select(f => new { f.FuelEntry.Date, f.Amount, f.Product.ProductName });

            var lubeSalesQuery = _context.LubeSales
                .Where(l => l.UsersId == userId && l.LubeEntry.Date >= fromDate)
                .Where(l => !productId.HasValue || l.ProductId == productId)
                .Where(l => !categoryId.HasValue || l.Product.ProductCategoryType.CategoryId == categoryId)
                .Select(l => new { l.LubeEntry.Date, l.Amount, l.Product.ProductName });

            var combined = await fuelSalesQuery.Concat(lubeSalesQuery).ToListAsync();

            var grouped = combined
                .GroupBy(x => new
                {
                    Product = x.ProductName,
                    Period = range switch
                    {
                        "month" => x.Date.ToString("yyyy-MM"),
                        "quarter" => $"{x.Date.Year}-Q{(x.Date.Month - 1) / 3 + 1}",
                        "year" => x.Date.Year.ToString(),
                        _ => x.Date.ToString("yyyy-MM-dd")
                    }
                })
                .Select(g => new SalesChartViewModel
                {
                    Product = g.Key.Product,
                    Period = g.Key.Period,
                    Total = g.Sum(x => x.Amount)
                })
                .ToList();

            return grouped;
        }
    }
}
