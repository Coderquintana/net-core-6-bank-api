using BankRestAPI.Data;
using BankRestAPI.DTOs;
using BankRestAPI.Models;
using BankRestAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankRestAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]s")]
    public class AccountController : Controller
    {
        private readonly BankDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;
        private readonly AccountService _accountService;
        private readonly BankService _bankService;
        private readonly CustomerService _customerService;

        public AccountController(BankDbContext dbContext, ILogger<AccountController> logger, AccountService accountService, BankService bankService, CustomerService customerService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _accountService = accountService;
            _bankService = bankService;
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            return Ok(await _accountService.GetAll());
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAccount(Guid id)
        {
            var account = await _accountService.GetById(id);

            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }


        [HttpPost]
        public async Task<IActionResult> AddAccount(AccountDTO accountDto)
        {
            try
            {
                Account account = new Account();

                if (ContainsNullOrEmpty(accountDto) || AccountExists(accountDto))
                {
                    return BadRequest();
                }

                if (account.Balance < 0)
                {
                    return BadRequest("El saldo de la cuenta no puede ser negativo");
                }

                var Customer = await _customerService.GetById(accountDto.Customer);
                var Bank = await _bankService.GetByCode(accountDto.Bank);

                if (Bank == null)
                {
                    return BadRequest("Bank Not Found");
                }
                if (Customer == null)
                {
                    return BadRequest("Customer Not Found");
                }
                
                account.Number = accountDto.Number;
                account.Currency = accountDto.Currency;
                account.Balance = accountDto.Balance;   
                account.Customer = Customer;
                account.Bank = Bank;

                await _accountService.Create(account);
                return Ok(account);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

   

        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteAccount(string code)
        {
            var account = await _accountService.GetByNumber(code);

            if (account == null) { return NotFound($"Account with id {code} not found"); }

            await _accountService.Delete(account.Id);

            return Ok(await _accountService.GetAll());
        }

        private bool ContainsNullOrEmpty(AccountDTO account)
        {
            if (account == null)
            {
                _logger.LogError("Account is null");
                return true;
            }
            if (string.IsNullOrEmpty(account.Currency))
            {
                _logger.LogError("AccountCurrrency is null or empty");
                return true;
            }
            if (account.Customer == null)
            {
                _logger.LogError("Customer is null or empty");
                return true;
            }
            if (account.Bank == null)
            {
                _logger.LogError("Bank is null");
                return true;
            }

            return false;
        }

        private bool AccountExists(AccountDTO account)
        {
            if (_dbContext.Account.Any(a => a.Number == account.Number))
            {
                _logger.LogError($"Account with Number {account.Number} already exists");
                return true;
            }
            return false;
        }
    }
}
