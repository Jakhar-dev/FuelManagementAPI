using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FuelManagementAPI.Repositories
{
    public class FuelEntryRepository : Repository<FuelEntry>, IFuelEntryRepository
    {
        private readonly FuelDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public FuelEntryRepository(FuelDbContext context, IHttpContextAccessor contextAccessor) : base(context)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        public async Task<FuelEntry> AddAsync(FuelEntry fuelEntry)
        {
            var userId = GetCurrentUserId();

            fuelEntry.UsersId = userId;

            foreach (var sale in fuelEntry.Sales)
            {
                sale.UsersId = userId;
            }

            _context.FuelEntries.Add(fuelEntry);
            await _context.SaveChangesAsync();
            return fuelEntry;
        }

        public async Task<List<FuelEntry>> GetAllAsync()
        {
            var userId = GetCurrentUserId();

            return await _context.FuelEntries
                .Where(f => f.UsersId == userId)
                .Include(f => f.Sales)
                .ThenInclude(s => s.Product)
                .ToListAsync();
        }

        public async Task<FuelEntry?> GetByIdAsync(int id)
        {
            var userId = GetCurrentUserId();

            return await _context.FuelEntries
                .Include(f => f.Sales)
                .FirstOrDefaultAsync(f => f.FuelEntryId == id && f.UsersId == userId);
        }

        public async Task<IEnumerable<FuelEntry>> GetFuelSaleAsync(int id)
        {
            var userId = GetCurrentUserId();

            return await _context.FuelEntries
                .Where(s => s.FuelEntryId == id && s.UsersId == userId)
                .ToListAsync();
        }

        public async Task<decimal> GetPreviousReadingAsync(int productId, DateTime entryDate)
        {
            var userId = GetCurrentUserId();

            var previousEntry = await _context.FuelEntries
                .Where(fe => fe.Date < entryDate && fe.UsersId == userId)
                .OrderByDescending(fe => fe.Date)
                .FirstOrDefaultAsync();

            if (previousEntry == null) return 0;

            var previousSale = await _context.FuelSales
                .FirstOrDefaultAsync(fs =>
                    fs.ProductId == productId &&
                    fs.FuelEntryId == previousEntry.FuelEntryId &&
                    fs.UsersId == userId);

            return previousSale?.CurrentReading ?? 0;
        }

        public async Task<List<FuelSale>> GetFuelSalesByDateAsync(DateTime date)
        {
            var userId = GetCurrentUserId();

            return await _context.FuelSales
                .Include(fs => fs.FuelEntry)
                .Where(fs => fs.FuelEntry.Date == date && fs.UsersId == userId)
                .ToListAsync();
        }

        public async Task<FuelEntry?> GetFuelEntryByDateAsync(DateTime date)
        {
            var userId = GetCurrentUserId();

            return await _context.FuelEntries
                .Include(e => e.Sales)
                .FirstOrDefaultAsync(e => e.Date.Date == date.Date && e.UsersId == userId);
        }

        public async Task DeleteAsync(int id)
        {
            var entry = await GetByIdAsync(id);
            if (entry != null)
            {
                _context.FuelEntries.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SaleExistsAsync(int productId, DateTime date)
        {
            var userId = GetCurrentUserId();

            return await _context.FuelEntries
                .Where(e => e.Date.Date == date.Date && e.UsersId == userId)
                .SelectMany(e => e.Sales)
                .AnyAsync(s => s.ProductId == productId);
        }

        public Task DeleteAsync(FuelEntry fuelEntry)
        {
            throw new NotImplementedException();
        }
    }
}
