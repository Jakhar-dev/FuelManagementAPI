using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface ILubeEntryRepository : IRepository<LubeEntry>
    {
        Task<List<LubeEntry>> GetAllAsync();
        Task<LubeEntry> AddAsync(LubeEntry lubeEntry);
        Task DeleteAsync(LubeEntry lubeEntry);

    }
}
