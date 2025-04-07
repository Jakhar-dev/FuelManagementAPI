using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuelManagementAPI.Repositories
{
    public class AttendantRepository : Repository<Attendant>, IAttendantRepository
    {
        private readonly FuelDbContext _context;

        public AttendantRepository(FuelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Attendant>> GetActiveAttendantsAsync()
        {
            return await _context.Attendants.Where(a => a.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Attendant>> GetAllAsync()
        {
            return await _context.Attendants.ToListAsync();
        }

        public async Task<Attendant> GetByIdAsync(int id)
        {
            return await _context.Attendants.FindAsync(id);
        }

        public async Task<Attendant> AddAsync(Attendant attendant)
        {
            _context.Attendants.Add(attendant);
            await _context.SaveChangesAsync();
            return attendant;
        }

        public async Task UpdateAsync(Attendant attendant)
        {
            _context.Entry(attendant).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var attendant = await _context.Attendants.FindAsync(id);
            if (attendant != null)
            {
                _context.Attendants.Remove(attendant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
