using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/accounts")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepo;

    public AccountController(IAccountRepository AccountRepository)
    {
        _accountRepo = AccountRepository;
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

    // Get Account By ID
    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetAccountById(int accountId)
    {
        var account = await _accountRepo.GetAccountByIdAsync(accountId);
        if (account == null) return NotFound("Account not found!");
        return Ok(account);
    }

    // Add Debit/Credit Transaction    
    [HttpPost("transaction")]
    public async Task<IActionResult> AddTransaction([FromBody] AccountTransactionViewModel model)
    {
        if (model.Amount <= 0) return BadRequest("Amount must be greater than zero.");

        var transaction = new AccountTransaction
        {
            AccountId = model.AccountId,
            Amount = model.Amount,
            TransactionType = model.TransactionType,
            TransactionDate = model.TransactionDate,
            Description = model.Description
        };

        var addedTransaction = await _accountRepo.AddTransactionAsync(transaction);
        return Ok(addedTransaction);
    }
}
