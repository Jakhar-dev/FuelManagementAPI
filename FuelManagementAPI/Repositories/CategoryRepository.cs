using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Models.ViewModal;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FuelManagementAPI.Repositories
{
    public class CategoryRepository : Repository<ProductCategory>, ICategoryRepository
    {
        private readonly FuelDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryRepository(FuelDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        public async Task<bool> CategoryExistsAsync(string categoryName)
        {
            var userId = GetCurrentUserId();
            return await _context.ProductCategories
                .AnyAsync(c => c.UsersId == userId && c.CategoryName.ToLower() == categoryName.ToLower());
        }

        public async Task<ProductCategory> AddCategoryAsync(CategoryViewModel model)
        {
            var userId = GetCurrentUserId();
            var category = new ProductCategory
            {
                CategoryName = model.CategoryName,
                Description = model.Description,
                UsersId = userId
                // UsersId will be auto-assigned by DbContext.SaveChangesAsync
            };

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync()
        {
            var userId = GetCurrentUserId();
            return await _context.ProductCategories
                .Where(c => c.UsersId == userId)
                .ToListAsync();
        }

        public async Task<ProductCategory?> GetCategoryByIdAsync(int id)
        {
            var userId = GetCurrentUserId();
            return await _context.ProductCategories
                .FirstOrDefaultAsync(c => c.CategoryId == id && c.UsersId == userId);
        }

        public async Task<ProductCategory?> GetCategoryByNameAsync(string name)
        {
            var userId = GetCurrentUserId();
            return await _context.ProductCategories
                .FirstOrDefaultAsync(c => c.UsersId == userId && c.CategoryName.ToLower() == name.ToLower());
        }
        public async Task AddCategoriesAsync(IEnumerable<ProductCategory> categories)
        {
            if (categories == null || !categories.Any())
                return;

            var userId = categories.First().UsersId;

            // Filter out any categories that already exist for this user
            var existingCategoryNames = await _context.ProductCategories
                .Where(c => c.UsersId == userId)
                .Select(c => c.CategoryName.ToLower())
                .ToListAsync();

            var newCategories = categories
                .Where(c => !existingCategoryNames.Contains(c.CategoryName.ToLower()))
                .ToList();

            if (newCategories.Any())
            {
                await _context.ProductCategories.AddRangeAsync(newCategories);
                await _context.SaveChangesAsync();
            }
        }
    }
}
