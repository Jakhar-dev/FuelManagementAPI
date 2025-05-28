using FuelManagementAPI.Models;
using FuelManagementAPI.ViewModels;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<List<Account>> GetAccountsAsync();
      //  Task<decimal> GetBalanceAsync(string CustomerName);
        Task<Account?> GetAccountByIdAsync(int accountId);
        Task<AccountTransaction> AddTransactionAsync(AccountTransaction transaction);       
        Task<IEnumerable<Account>> CreateAccountsAsync(IEnumerable<Account> accounts);
        Task<List<Account>> GetAllWithTransactionsAsync();
        Task AddMultipleTransactionsAsync(List<AccountTransaction> transactions);
        Task<IEnumerable<CustomerLedgerViewModel>> GetCustomerLedgerAsync();
    }
}
