using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using FuelManagementAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
[Route("api/accounts")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepo;
    private readonly ITransactionRepository _transactionRepository;

    public AccountController(IAccountRepository AccountRepository, ITransactionRepository transactionRepository)
    {
        _accountRepo = AccountRepository;
        _transactionRepository = transactionRepository;
    }

    // Create Account
    [HttpPost("create-accounts")]
    public async Task<IActionResult> CreateMultipleAccounts([FromBody] List<AccountViewModel> models)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var accounts = models.Select(model => new Account
        {
            CustomerName = model.CustomerName,
            CustomerPhone = model.CustomerPhone,
            CustomerAdress = model.CustomerAddress,
            Description = model.Description,
        }).ToList();

        var createdAccounts = await _accountRepo.CreateAccountsAsync(accounts);
        return Ok(createdAccounts);
    }

    // Get All Accounts
    [HttpGet("getAccounts")]
    public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
    {
        try
        {
            var accounts = await _accountRepo.GetAccountsAsync();
                        
            return Ok(accounts);
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching accounts", error = ex.Message });
        }
    }

    [HttpGet("ledger")]
    public async Task<ActionResult<IEnumerable<CustomerLedgerViewModel>>> GetCustomerLedger()
    {
        var result = await _accountRepo.GetCustomerLedgerAsync();
        return Ok(result);
    }

    // Get Account By ID
    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetAccountById(int accountId)
    {
        var account = await _accountRepo.GetAccountByIdAsync(accountId);
        if (account == null) return NotFound("Account not found!");
        return Ok(account);
    }

    // Add Debit/Credit Transaction    
    [HttpPost("transactions")] // Plural
    public async Task<IActionResult> AddTransactions([FromBody] List<AccountTransactionViewModel> models)
    {
        if (models == null || !models.Any())
            return BadRequest("No transactions provided.");

        var results = new List<AccountTransaction>();

        foreach (var model in models)
        {
            if (model.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            var transaction = new AccountTransaction
            {
                AccountId = model.AccountId,
                Amount = model.Amount,
                TransactionType = model.TransactionType,
                TransactionDate = model.TransactionDate,
                Description = model.Description
            };

            var addedTransaction = await _accountRepo.AddTransactionAsync(transaction);
            results.Add(addedTransaction);
        }
        return Ok(results);
    }

    [HttpGet("get-transaction")]
    public async Task<ActionResult> GetAllTransaction()
    {
        var accounts = await _accountRepo.GetAllWithTransactionsAsync();

        var result = accounts
            .Where(a => a.Transactions != null && a.Transactions.Any())
            .SelectMany(a => a.Transactions.Select(t => new AccountTransactionViewModel
            {
                TransactionId = t.TransactionId,
                AccountId = a.AccountId,
                CustomerName = a.CustomerName, // 👈 map customer name from Account
                Amount = (decimal)t.Amount,
                TransactionType = t.TransactionType,
                Description = t.Description,
                TransactionDate = t.TransactionDate
            }))
            .ToList();

        return Ok(result);
    }

    //[HttpGet("AllTransaction")]
    //public async Task<IActionResult> SearchTransactions([FromQuery] string? customerName)
    //{
    //    var result = await _accountRepo.GetTransactionsAsync(customerName);
    //    return Ok(result);
    //}


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }

        await _transactionRepository.DeleteAsync(transaction);
        return NoContent();
    }

}
