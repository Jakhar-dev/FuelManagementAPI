using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FuelManagementAPI.Repositories
{
    public class TransactionRepository : Repository<AccountTransaction>, ITransactionRepository
    {
        private readonly FuelDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public TransactionRepository(FuelDbContext context, IHttpContextAccessor contextAccessor) : base(context)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        public async Task<List<AccountTransaction>> GetAllTransactions()
        {
            var userId = GetCurrentUserId();
            return await _context.AccountTransactions
                .Where(t => t.UsersId == userId) // Make sure AccountTransaction has UsersId
                .ToListAsync();
        }

        public async Task DeleteAsync(AccountTransaction transaction)
        {
            _context.AccountTransactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<AccountTransaction> GetByIdAsync(int id)
        {
            var userId = GetCurrentUserId();
            return await _context.AccountTransactions
                .FirstOrDefaultAsync(t => t.TransactionId == id && t.UsersId == userId);
        }
    }
}
