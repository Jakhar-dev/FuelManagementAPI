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

        public async Task<IEnumerable<Product>> GetProductsByCategoryNameAsync(string categoryName)
        {
            var userId = GetCurrentUserId();
            return await _context.Products
                .Include(p => p.ProductCategory)
                .Where(p => p.UsersId == userId && p.ProductCategory.CategoryName.ToLower() == categoryName.ToLower())
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var userId = GetCurrentUserId();
            return await _context.Products
                .Include(p => p.ProductCategory)
                .Where(p => p.CategoryId == categoryId && p.UsersId == userId)
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
    }
}
