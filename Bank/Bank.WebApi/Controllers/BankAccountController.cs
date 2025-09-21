using Microsoft.AspNetCore.Mvc;
using Bank.Domain.Models;

namespace Bank.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankAccountController : ControllerBase
{
    private static Dictionary<string, BankAccount> _accounts = new Dictionary<string, BankAccount>();
    private readonly ILogger<BankAccountController> _logger;

    public BankAccountController(ILogger<BankAccountController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets all bank accounts
    /// </summary>
    /// <returns>List of bank accounts</returns>
    [HttpGet]
    public ActionResult<IEnumerable<object>> GetAccounts()
    {
        var accounts = _accounts.Select(kvp => new 
        { 
            AccountNumber = kvp.Key, 
            CustomerName = kvp.Value.CustomerName, 
            Balance = kvp.Value.Balance 
        });
        return Ok(accounts);
    }

    /// <summary>
    /// Gets a specific bank account by account number
    /// </summary>
    /// <param name="accountNumber">Account number</param>
    /// <returns>Bank account</returns>
    [HttpGet("{accountNumber}")]
    public ActionResult<object> GetAccount(string accountNumber)
    {
        if (!_accounts.ContainsKey(accountNumber))
        {
            return NotFound();
        }
        
        var account = _accounts[accountNumber];
        return Ok(new 
        { 
            AccountNumber = accountNumber, 
            CustomerName = account.CustomerName, 
            Balance = account.Balance 
        });
    }

    /// <summary>
    /// Creates a new bank account
    /// </summary>
    /// <param name="request">Bank account creation request</param>
    /// <returns>Created bank account</returns>
    [HttpPost]
    public ActionResult<object> CreateAccount([FromBody] CreateAccountRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.AccountNumber) || string.IsNullOrEmpty(request.CustomerName))
        {
            return BadRequest("AccountNumber and CustomerName are required");
        }

        if (_accounts.ContainsKey(request.AccountNumber))
        {
            return Conflict("Account already exists");
        }

        var account = new BankAccount(request.CustomerName, request.InitialBalance);
        _accounts[request.AccountNumber] = account;
        
        return CreatedAtAction(nameof(GetAccount), new { accountNumber = request.AccountNumber }, new 
        { 
            AccountNumber = request.AccountNumber, 
            CustomerName = account.CustomerName, 
            Balance = account.Balance 
        });
    }

    /// <summary>
    /// Performs a debit operation on a bank account
    /// </summary>
    /// <param name="accountNumber">Account number</param>
    /// <param name="request">Debit request</param>
    /// <returns>Updated account</returns>
    [HttpPost("{accountNumber}/debit")]
    public ActionResult<object> Debit(string accountNumber, [FromBody] AmountRequest request)
    {
        if (!_accounts.ContainsKey(accountNumber))
        {
            return NotFound();
        }

        try
        {
            var account = _accounts[accountNumber];
            account.Debit(request.Amount);
            return Ok(new 
            { 
                AccountNumber = accountNumber, 
                CustomerName = account.CustomerName, 
                Balance = account.Balance 
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Performs a credit operation on a bank account
    /// </summary>
    /// <param name="accountNumber">Account number</param>
    /// <param name="request">Credit request</param>
    /// <returns>Updated account</returns>
    [HttpPost("{accountNumber}/credit")]
    public ActionResult<object> Credit(string accountNumber, [FromBody] AmountRequest request)
    {
        if (!_accounts.ContainsKey(accountNumber))
        {
            return NotFound();
        }

        try
        {
            var account = _accounts[accountNumber];
            account.Credit(request.Amount);
            return Ok(new 
            { 
                AccountNumber = accountNumber, 
                CustomerName = account.CustomerName, 
                Balance = account.Balance 
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class CreateAccountRequest
{
    public string AccountNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public double InitialBalance { get; set; } = 0;
}

public class AmountRequest
{
    public double Amount { get; set; }
}