using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface IFuelPurchaseRepository
    {
        Task AddAsync(FuelPurchase fuelPurchase);
    //    Task UpdateCategoryPriceAsync(int categoryTypeId, decimal price);
    }


}
