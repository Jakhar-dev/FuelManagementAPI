using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementAPI.Repositories
{
    public class FuelEntryRepository : Repository<FuelEntry>, IFuelEntryRepository
    {
        private readonly FuelDbContext _FuelEntryContext;

        public FuelEntryRepository(FuelDbContext context) : base(context)
        {
            _FuelEntryContext = context;
        }

        public async Task<IEnumerable<FuelEntry>> GetFuelSaleAsync(int id)
        {
            return await _FuelEntryContext.FuelEntries
                            .Where(s => s.FuelEntryId == id)
                            .ToListAsync();
        }
        public async Task<FuelEntry> AddAsync(FuelEntry fuelEntry)
        {
            _FuelEntryContext.FuelEntries.Add(fuelEntry);
            await _FuelEntryContext.SaveChangesAsync();
            return fuelEntry;
        }

        public async Task<FuelEntry?> GetByIdAsync(int id)
        {
            return await _FuelEntryContext.FuelEntries
                .Include(f => f.Sales)
                .FirstOrDefaultAsync(f => f.FuelEntryId == id);
        }

        public async Task<List<FuelEntry>> GetAllAsync()
        {
            return await _FuelEntryContext.FuelEntries
                .Include(f => f.Sales)
                .ThenInclude(s => s.Product)
                .ToListAsync();
        }

        // Get previous reading from FuelEntry
        public async Task<decimal> GetPreviousReadingAsync(int productId, DateTime entryDate)
        {
            var previousEntry = await _FuelEntryContext.FuelEntries
                .Where(fe => fe.Date < entryDate)
                .OrderByDescending(fe => fe.Date)
                .FirstOrDefaultAsync();

            if (previousEntry == null) return 0;

            var previousSale = await _FuelEntryContext.FuelSales
                .FirstOrDefaultAsync(fs =>
                    fs.FuelSaleId == productId &&
                    fs.FuelEntryId == previousEntry.FuelEntryId);

            return previousSale?.CurrentReading ?? 0;
        }

        // Include related entities when fetching
        public async Task<List<FuelSale>> GetFuelSalesByDateAsync(DateTime date)
        {
            return await _FuelEntryContext.FuelSales
                .Include(fs => fs.FuelEntry)
                .Include(fs => fs.FuelEntryId)
                .Where(fs => fs.FuelEntry.Date == date)
                .ToListAsync();
        }

        public async Task<FuelEntry> GetFuelEntryByDateAsync(DateTime date)
        {
            return await _FuelEntryContext.FuelEntries
                .Include(e => e.Sales)
                .FirstOrDefaultAsync(e => e.Date.Date == date.Date);
        }

        public Task DeleteAsync(FuelEntry fuelEntry)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> SaleExistsAsync(int productId, DateTime date)
        {
            return await _FuelEntryContext.FuelEntries
                .Where(e => e.Date.Date == date.Date)
                .SelectMany(e => e.Sales)
                .AnyAsync(s => s.ProductId == productId);
        }
    }
}
