using FuelManagementAPI.Models;
using FuelManagementAPI.Models.ViewModal;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface ICategoryTypeRepository : IRepository<ProductCategoryType>
    {
        Task<IEnumerable<ProductCategoryType>> GetAllCategoryTypeAsync();
        Task<IEnumerable<ProductCategoryType?>> GetCategoryTypeByCategoryIdAsync(int id);
        Task<IEnumerable<ProductCategoryType>> GetCategoryTypeByNameAsync(string name);
        Task<List<ProductCategoryType>> AddCategoryTypeAsync(List<CategoryTypeViewModel> model);
        Task<bool> CategoryTypeExistsAsync(string categoryName);


    }
}
