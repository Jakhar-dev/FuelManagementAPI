using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FuelManagementAPI.Repositories
{
    public class LubeEntryRepository : Repository<LubeEntry>, ILubeEntryRepository
    {
        private readonly FuelDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public LubeEntryRepository(FuelDbContext context, IHttpContextAccessor contextAccessor) : base(context)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        public async Task<LubeEntry> AddAsync(LubeEntry lubeEntry)
        {
            var userId = GetCurrentUserId();

            lubeEntry.UsersId = userId;

            foreach (var sale in lubeEntry.Sales)
            {
                sale.UsersId = userId;
            }

            _context.LubeEntries.Add(lubeEntry);
            await _context.SaveChangesAsync();
            return lubeEntry;
        }

        public async Task<bool> SaleExistsAsync(int productId, DateTime date)
        {
            var userId = GetCurrentUserId();

            return await _context.LubeEntries
                .Where(e => e.Date.Date == date.Date && e.UsersId == userId)
                .SelectMany(e => e.Sales)
                .AnyAsync(s => s.ProductId == productId);
        }

        public async Task<List<LubeEntry>> GetAllAsync()
        {
            var userId = GetCurrentUserId();

            return await _context.LubeEntries
                .Where(le => le.UsersId == userId)
                .Include(le => le.Sales)
                .ThenInclude(s => s.Product)
                .ToListAsync();
        }

        public async Task<LubeEntry?> GetByIdAsync(int id)
        {
            var userId = GetCurrentUserId();

            return await _context.LubeEntries
                .Include(le => le.Sales)
                .ThenInclude(s => s.Product)
                .FirstOrDefaultAsync(le => le.LubeEntryId == id && le.UsersId == userId);
        }

        public async Task DeleteAsync(LubeEntry lubeEntry)
        {
            _context.LubeEntries.Remove(lubeEntry);
            await _context.SaveChangesAsync();
        }

        // Not yet implemented
        public Task AddAsync(LubeSale entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(LubeSale entity)
        {
            throw new NotImplementedException();
        }
    }
}
