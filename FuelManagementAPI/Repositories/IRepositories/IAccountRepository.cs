using FuelManagementAPI.Models;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<List<Account>> GetAccountsAsync();
      //  Task<decimal> GetBalanceAsync(string CustomerName);
        Task<Account?> GetAccountByIdAsync(int accountId);
        Task<AccountTransaction> AddTransactionAsync(AccountTransaction transaction);       
        Task<IEnumerable<Account>> CreateAccountsAsync(IEnumerable<Account> accounts);
    }
}
