using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface ILubeSalesRepository : IRepository<LubeSale>
    {
        Task DeleteAsync(LubeSale sale);
        Task<LubeSale> GetByIdAsync(int id);
    }
}
