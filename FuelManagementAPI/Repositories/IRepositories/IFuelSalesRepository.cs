using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface IFuelSalesRepository : IRepository<FuelSale>
    {
        Task<FuelSale?> GetLastSaleForProductAsync(int productId);
        Task AddFuelSaleAsync(FuelSale sale);
        Task DeleteAsync(FuelSale fuelSale);
    }
}
