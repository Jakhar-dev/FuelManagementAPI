using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    private readonly FuelDbContext _context;

    public AccountRepository(FuelDbContext context) : base(context)
    {
        _context = context;
    }

    // Create New Account
   public async Task<IEnumerable<Account>> CreateAccountsAsync(IEnumerable<Account> accounts)
    {
        _context.Accounts.AddRange(accounts);
        await _context.SaveChangesAsync();
        return accounts;
    }


    // Get All Accounts
    public async Task<List<Account>> GetAccountsAsync()
    {
        return await _context.Accounts.ToListAsync();
    }

    // Find Account by ID
    public async Task<Account?> GetAccountByIdAsync(int accountId)
    {
        return await _context.Accounts.Include(a => a.Transactions).FirstOrDefaultAsync(a => a.AccountId == accountId);
    }

    // Add Transaction (Debit/Credit)
    public async Task<AccountTransaction> AddTransactionAsync(AccountTransaction transaction)
    {
        var account = await _context.Accounts.FindAsync(transaction.AccountId);
        if (account == null) throw new Exception("Account not found!");

        if (transaction.TransactionType == "Debit")
            account.TotalDebit += transaction.Amount;
        else if (transaction.TransactionType == "Credit")
            account.TotalCredit += transaction.Amount;

        _context.AccountTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

}
