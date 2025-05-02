using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FuelManagementAPI.Repositories
{
    public class AttendantRepository : Repository<Attendant>, IAttendantRepository
    {
        private readonly FuelDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public AttendantRepository(FuelDbContext context, IHttpContextAccessor contextAccessor) : base(context)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        public async Task<IEnumerable<Attendant>> GetActiveAttendantsAsync()
        {
            var userId = GetCurrentUserId();
            return await _context.Attendants
                .Where(a => a.IsActive && a.UsersId == userId) // Assumes Attendant has UsersId
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendant>> GetAllAsync()
        {
            var userId = GetCurrentUserId();
            return await _context.Attendants
                .Where(a => a.UsersId == userId)
                .ToListAsync();
        }

        public async Task<Attendant> GetByIdAsync(int id)
        {
            var userId = GetCurrentUserId();
            return await _context.Attendants
                .FirstOrDefaultAsync(a => a.AttendantId == id && a.UsersId == userId);
        }

        public async Task<Attendant> AddAsync(Attendant attendant)
        {
            attendant.UsersId = GetCurrentUserId(); // 👈 Ensure user context is set
            _context.Attendants.Add(attendant);
            await _context.SaveChangesAsync();
            return attendant;
        }

        public async Task UpdateAsync(Attendant attendant)
        {
            attendant.UsersId = GetCurrentUserId(); // Optional: reinforce user binding
            _context.Entry(attendant).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var userId = GetCurrentUserId();
            var attendant = await _context.Attendants
                .FirstOrDefaultAsync(a => a.AttendantId == id && a.UsersId == userId);

            if (attendant != null)
            {
                _context.Attendants.Remove(attendant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
