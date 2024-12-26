using Microsoft.AspNetCore.Mvc;
using BankingPlatformAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class BankAccountController : ControllerBase
{
    private readonly BankingDbContext _context;

    public BankAccountController(BankingDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAllAccounts()
    {
        return Ok(_context.BankAccounts.ToList());
    }

    [HttpPost]
    public IActionResult CreateAccount(BankAccount account)
    {
        _context.BankAccounts.Add(account);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetAllAccounts), new { id = account.Id }, account);
    }
}
