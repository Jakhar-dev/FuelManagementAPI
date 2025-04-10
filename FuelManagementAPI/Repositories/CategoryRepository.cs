using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Models.ViewModal;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementAPI.Repositories
{
    public class CategoryRepository : Repository<ProductCategory>, ICategoryRepository
    {
        private readonly FuelDbContext _context;

        public CategoryRepository(FuelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CategoryExistsAsync(string categoryName)
        {
            return await _context.ProductCategories
                .AnyAsync(c => c.CategoryName.ToLower() == categoryName.ToLower());
        }

        public async Task<ProductCategory> AddCategoryAsync(CategoryViewModel model)
        {
            var category = new ProductCategory
            {
                CategoryName = model.CategoryName,
                Description = model.Description
            };

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        public async Task<ProductCategory?> GetCategoryByIdAsync(int id)
        {
            return await _context.ProductCategories.FindAsync(id);
        }

        public async Task<ProductCategory?> GetCategoryByNameAsync(string name)
        {
            return await _context.ProductCategories.FirstOrDefaultAsync(c => c.CategoryName.ToLower() == name.ToLower());
        }           
    }
}
