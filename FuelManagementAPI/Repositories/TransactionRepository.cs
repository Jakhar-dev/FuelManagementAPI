using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementAPI.Repositories
{
    public class TransactionRepository : Repository<AccountTransaction>, ITransactionRepository
    {
        private readonly FuelDbContext _context;
        public TransactionRepository(FuelDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<AccountTransaction>> GetAllTransactions()
        {
            return await _context.AccountTransactions.ToListAsync();
        }
        public async Task DeleteAsync(AccountTransaction transaction)
        {
            _context.AccountTransactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
        public async Task<AccountTransaction> GetByIdAsync(int id)
        {
            return await _context.AccountTransactions
                .FirstOrDefaultAsync(t => t.TransactionId == id);
        }

    }
}
