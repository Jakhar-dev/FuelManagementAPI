using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementAPI.Repositories
{
    public class LubeEntryRepository : Repository<LubeEntry>, ILubeEntryRepository
    {
        private readonly FuelDbContext _context;

        public LubeEntryRepository(FuelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<LubeEntry> AddAsync(LubeEntry lubeEntry)
        {
            _context.LubeEntries.Add(lubeEntry);
            await _context.SaveChangesAsync();
            return lubeEntry;
        }

        public Task AddAsync(LubeSale entity)
        {
            throw new NotImplementedException();
        }

        public async Task<List<LubeEntry>> GetAllAsync()
        {
            return await _context.LubeEntries
                .Include(l => l.Sales)
                .ThenInclude(s => s.Product)
                .ToListAsync();
        }

        public Task UpdateAsync(LubeSale entity)
        {
            throw new NotImplementedException();
        }

        Task<LubeEntry> IRepository<LubeEntry>.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }       
    }  
}
