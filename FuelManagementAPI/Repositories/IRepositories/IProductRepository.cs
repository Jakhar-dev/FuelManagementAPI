using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface IProductRepository : IRepository<Product>
    {       
        Task<Product> GetAsync(int id);
        Task<List<Product>> GetProductsAsync();
        Task AddProductAsync(Product product);
        Task AddProductsAsync(IEnumerable<Product> products); // For bulk operations
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<ProductCategory?> GetCategoryByNameAsync(string categoryName);
        Task<bool> ProductExistsAsync(string productName);


    }
}