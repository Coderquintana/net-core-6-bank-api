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
        // Service Injection, logger and constructors
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

        // api/v1/Accounts ------------------- GET -------------------
        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            return Ok(await _accountService.GetAll());
        }

        // api/v1/Accounts/{number} ------------------- GET BY ACCOUNT NUMBER -------------------
        [HttpGet("{number}")]
        public async Task<IActionResult> GetAccount(string number)
        {
            var account = await _accountService.GetByNumber(number);

            if (account == null)
            {
                return NotFound("Account Not Found");
            }
            return Ok(account);
        }

        // api/v1/Accounts ------------------- POST -------------------
        [HttpPost]
        public async Task<IActionResult> AddAccount(AccountDTO accountDto)
        {
            try
            {
                Account account = new Account();
                string result = string.Empty;

                result = ContainsNullOrEmpty(accountDto);
                if (!result.Equals("valid"))
                {
                    return BadRequest(result);
                }

                result = AccountIsValid(accountDto);
                if(!result.Equals("valid")) 
                {
                    return BadRequest(result);
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

        // api/v1/Account/{number}/{amount} ------------------- PUT -------------------
        [HttpPut]
        [Route("{number}/{amount}")]
        public async Task<IActionResult> UpdateBalance(string number, decimal amount)
        {
            var account = await _accountService.GetByNumber(number);
            if (account  == null)
            {
                return NotFound("Account Not Found");
            }
            if (amount <= 0)
            {
                return BadRequest("Amount negative or zero not valid");
            }
            account.Balance = amount;
            await _accountService.Update(account);
            return Ok(await _accountService.GetById(account.Id));
        }

        // api/v1/Accounts ------------------- DELETE-------------------
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteAccount(string code)
        {
            var account = await _accountService.GetByNumber(code);

            if (account == null) { return NotFound($"Account with id {code} not found"); }

            await _accountService.Delete(account.Id);

            return Ok(await _accountService.GetAll());
        }

        // ------------------- My New Account Null or Empty Validations -------------------
        private string ContainsNullOrEmpty(AccountDTO account)
        {
            if (account == null)
            {
                _logger.LogError("Account is null");
                return "Account is null";
            }
            if (string.IsNullOrEmpty(account.Currency))
            {
                _logger.LogError("AccountCurrrency is null or empty");
                return "AccountCurrrency is null or empty";
            }
            if (account.Customer == null)
            {
                _logger.LogError("Customer is null or empty");
                return "Customer is null or empty";
            }
            if (account.Bank == null)
            {
                _logger.LogError("Bank is null");
                return "Bank is null";
            }

            return "valid";
        }

        // ------------------- My Account Validations -------------------
        private string AccountIsValid(AccountDTO account)
        {
            if (_dbContext.Account.Any(a => a.Number == account.Number))
            {
                _logger.LogError($"Account with Number {account.Number} already exists");
                return $"Account with Number {account.Number} already exists";
            }
            if (account.Balance <= 0)
            {
                _logger.LogError("The balance cannot be negative or zero");
                return "The balance cannot be negative or zero";
            }
            return "valid";
        }
    }
}
