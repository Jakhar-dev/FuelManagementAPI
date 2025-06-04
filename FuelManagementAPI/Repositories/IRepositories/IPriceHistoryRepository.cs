using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface IPriceHistoryRepository : IRepository<PriceHistory>
    {
        Task<PriceHistory> GetByIdAsync(int id);
        PriceHistory Update(PriceHistory price);
        PriceHistory Delete(int id);
        Task Add(PriceHistory price);
        Task<bool> UpdateProductPricesAsync(PriceUpdateViewModel model);
        Task<PriceHistory> GetPriceForProductByDate(int productId, DateTime date);
        Task<List<PriceHistory>> GetPricesForCurrentUserAsync(int userId);

    }
}
