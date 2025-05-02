using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    private readonly FuelDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccountRepository(FuelDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var id) ? id : 0;
    }

    public async Task<IEnumerable<Account>> CreateAccountsAsync(IEnumerable<Account> accounts)
    {
        var userId = GetCurrentUserId();

        foreach (var account in accounts)
        {
            account.UsersId = userId;
        }

        _context.Accounts.AddRange(accounts);
        await _context.SaveChangesAsync();
        return accounts;
    }

    public async Task<List<Account>> GetAllWithTransactionsAsync()
    {
        var userId = GetCurrentUserId();

        return await _context.Accounts
            .Include(a => a.Transactions)
            .Where(a => a.UsersId == userId)
            .ToListAsync();
    }

    public async Task<List<Account>> GetAccountsAsync()
    {
        var userId = GetCurrentUserId();

        return await _context.Accounts
            .Where(a => a.UsersId == userId)
            .ToListAsync();
    }

    public async Task<Account?> GetAccountByIdAsync(int accountId)
    {
        var userId = GetCurrentUserId();

        return await _context.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.AccountId == accountId && a.UsersId == userId);
    }

    public async Task<AccountTransaction> AddTransactionAsync(AccountTransaction transaction)
    {
        var userId = GetCurrentUserId();

        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountId == transaction.AccountId && a.UsersId == userId);

        if (account == null)
            throw new Exception("Account not found!");

        transaction.UsersId = userId;

        if (transaction.TransactionType == "Debit")
            account.TotalDebit += transaction.Amount;
        else if (transaction.TransactionType == "Credit")
            account.TotalCredit += transaction.Amount;

        _context.AccountTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task AddMultipleTransactionsAsync(List<AccountTransaction> transactions)
    {
        _context.AccountTransactions.AddRange(transactions);
        await _context.SaveChangesAsync();
    }
}
