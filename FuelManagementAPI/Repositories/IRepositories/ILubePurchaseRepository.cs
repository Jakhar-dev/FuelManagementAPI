using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface ILubePurchaseRepository
    {
        Task AddAsync(LubePurchase lubePurchase);
      //  Task UpdateProductPriceAsync(int productId, decimal price);
    }


}
