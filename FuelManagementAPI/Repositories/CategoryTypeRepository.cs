using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Models.ViewModal;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FuelManagementAPI.Repositories
{
    public class CategoryTypeRepository : Repository<ProductCategoryType>, ICategoryTypeRepository
    {
        private readonly FuelDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryTypeRepository(FuelDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        public async Task<bool> CategoryTypeExistsAsync(string categoryTypeName)
        {
            var userId = GetCurrentUserId();
            return await _context.ProductCategoriesType
                .AnyAsync(c => c.UsersId == userId && c.CategoryTypeName.ToLower() == categoryTypeName.ToLower());
        }
               
        public async Task<IEnumerable<ProductCategoryType>> GetAllCategoryTypeAsync()
        {
            var userId = GetCurrentUserId();
            return await _context.ProductCategoriesType
                .Where(c => c.UsersId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductCategoryType>> GetCategoryTypeByNameAsync(string name)
        {
            var userId = GetCurrentUserId();
            return await _context.ProductCategoriesType
                .Include(c => c.productCategory)
                .Where(c => c.UsersId == userId && c.productCategory.CategoryName.ToLower() == name.ToLower())
                .ToListAsync();
        }

        public async Task<List<ProductCategoryType>> AddCategoryTypeAsync(List<CategoryTypeViewModel> models)
        {
            var userId = GetCurrentUserId();
            var addedCategoryTypes = new List<ProductCategoryType>();

            foreach (var model in models)
            {
                bool exists = await _context.ProductCategoriesType
                    .AnyAsync(c => c.UsersId == userId && c.CategoryTypeName.ToLower() == model.CategoryTypeName.ToLower());
                if (exists)
                {
                    throw new InvalidOperationException($"Category Type '{model.CategoryTypeName}' already exists");
                }

                var categorytype = new ProductCategoryType
                {
                    CategoryId = model.CategoryId,
                    CategoryTypeName = model.CategoryTypeName,
                    Description = model.Description,
                    UsersId = userId,
                };

                _context.ProductCategoriesType.Add(categorytype);
                addedCategoryTypes.Add(categorytype);
            }

            await _context.SaveChangesAsync();
            return addedCategoryTypes;
        }        
        public async Task<IEnumerable<ProductCategoryType?>> GetCategoryTypeByCategoryIdAsync(int id)
        {
            var userId = GetCurrentUserId();
            return await _context.ProductCategoriesType
                .Where(ct => ct.CategoryId == id && ct.UsersId == userId)
                .ToListAsync();
        }
    }
}
