using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementAPI.Repositories
{
    public class LubeSalesRepository : Repository<LubeSale>, ILubeSalesRepository
    {
        private readonly FuelDbContext _context;

        public LubeSalesRepository(FuelDbContext context) : base(context) 
        {
            _context = context;
        }
        public async Task<LubeSale> GetByIdAsync(int id)
        {
            return await _context.LubeSales
                .FirstOrDefaultAsync(s => s.LubeId == id); // ✅ Make sure it's LubeId
        }

        public async Task DeleteAsync(LubeSale sale)
        {
            _context.LubeSales.Remove(sale);
            await _context.SaveChangesAsync();
        }
    }
}
