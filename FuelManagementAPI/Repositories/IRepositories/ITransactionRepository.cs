using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface ITransactionRepository : IRepository<AccountTransaction>
    {
        Task<List<AccountTransaction>> GetAllTransactions();
        Task DeleteAsync(AccountTransaction accountTransaction);
        Task<AccountTransaction> GetByIdAsync(int id);
    }
}
