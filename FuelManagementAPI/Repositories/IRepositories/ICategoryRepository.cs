using FuelManagementAPI.Models;
using FuelManagementAPI.Models.ViewModal;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface ICategoryRepository : IRepository<ProductCategory>
    {
        Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync();
        Task<ProductCategory?> GetCategoryByIdAsync(int id);
        Task<ProductCategory?> GetCategoryByNameAsync(string name);
        Task<ProductCategory> AddCategoryAsync(CategoryViewModel model);
        Task<bool> CategoryExistsAsync(string categoryName);
    }
}
