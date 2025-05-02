using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FuelManagementAPI.Repositories
{
    public class LubeSalesRepository : Repository<LubeSale>, ILubeSalesRepository
    {
        private readonly FuelDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public LubeSalesRepository(FuelDbContext context, IHttpContextAccessor contextAccessor) : base(context)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        public async Task<LubeSale> GetByIdAsync(int id)
        {
            var userId = GetCurrentUserId();
            return await _context.LubeSales
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.LubeId == id && s.UsersId == userId);
        }

        public async Task DeleteAsync(LubeSale sale)
        {
            _context.LubeSales.Remove(sale);
            await _context.SaveChangesAsync();
        }
    }
}
