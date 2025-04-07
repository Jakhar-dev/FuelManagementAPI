using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementAPI.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly FuelDbContext _context;
        public ProductRepository(FuelDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task AddProductsAsync(IEnumerable<Product> products)
        {
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }

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

        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _context.Products
                .Where(p => p.ProductCategory == category)
                .ToListAsync();
        }  
        public Task<List<Product>> GetProductsAsync()
        {
            return _context.Products.ToListAsync();
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            return await _context.Products
                .Select(p => p.ProductCategory)
                .Distinct()
                .ToListAsync();
        }
    }
}
