using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface IPriceRepository : IRepository<Price>
    {
        Price Get(int id);
        Price Update(Price price);
        Price Delete(int id);
        void Add(Price price);
        Task<bool> UpdateProductPricesAsync(PriceUpdateViewModel model);
    }
}
