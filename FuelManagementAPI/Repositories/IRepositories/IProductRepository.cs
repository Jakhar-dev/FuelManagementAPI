using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface IProductRepository : IRepository<Product>
    {       
        Task<Product> GetAsync(int id);
        Task<List<Product>> GetProductsAsync();
        Task<ProductCategoryType> GetCategoryByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<ProductCategory?> GetCategoryByNameAsync(string categoryName);
        Task<bool> ProductExistsAsync(string productName);
        Task<IEnumerable<Product>> GetProductsByCategoryNameAsync(string categoryName);
        Task<IEnumerable<Product>> GetProductsByCategoryTypeIdAsync(int categoryTypeId);
        Task<List<SalesChartViewModel>> GetSalesChartByProductAsync(string range, int? categoryId, int? productId);
    }
}