using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface IPriceRepository : IRepository<Price>
    {
        Task<Price> GetByIdAsync(int id);
        Price Update(Price price);
        Price Delete(int id);
        void Add(Price price);
        Task<bool> UpdateProductPricesAsync(PriceUpdateViewModel model);
        Task<Price> GetLatestPriceForProductBeforeDate(int productId, DateTime date);
    }
}
