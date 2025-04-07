using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories
{
    public interface IAttendantRepository : IRepository<Attendant>
    {
        Task<IEnumerable<Attendant>> GetAllAsync();
        Task<Attendant> GetByIdAsync(int id);
        Task<Attendant> AddAsync(Attendant attendant);
        Task UpdateAsync(Attendant attendant);
        Task DeleteAsync(int id);
        Task<IEnumerable<Attendant>> GetActiveAttendantsAsync();
    }
}
