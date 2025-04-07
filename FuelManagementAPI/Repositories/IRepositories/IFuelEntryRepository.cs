using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories
{
    public interface IFuelEntryRepository : IRepository<FuelEntry>
    {    
       Task<IEnumerable<FuelEntry>> GetFuelSaleAsync(int id);
        Task<FuelEntry> AddAsync(FuelEntry fuelEntry);
        Task<FuelEntry?> GetByIdAsync(int id);
        Task<List<FuelEntry>> GetAllAsync();
        Task<Decimal> GetPreviousReadingAsync(int ProductId, DateTime entryDate);
        Task<List<FuelSale>> GetFuelSalesByDateAsync(DateTime date);
        Task<FuelEntry> GetFuelEntryByDateAsync(DateTime date);
    }
}
