using BankRestAPI.Data;
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
        private readonly IEntityService<Account> _accountService;
        private readonly IEntityService<Bank> _bankService;
        private readonly CustomerService _customerService;

        public AccountController(BankDbContext dbContext, ILogger<AccountController> logger, IEntityService<Account> accountService, IEntityService<Bank> bankService, CustomerService customerService)
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
        public async Task<IActionResult> AddAccount(Account account)
        {
            try
            {
                if (ContainsNullOrEmpty(account) || AccountExists(account) || !ValidateBankAndCustomer(account))
                {
                    return BadRequest();
                }

                if (account.Balance < 0)
                {
                    return BadRequest("El saldo de la cuenta no puede ser negativo");
                }

                account.Customer = await _customerService.GetById(account.CustomerDocumentNumber);
                account.Bank = await _bankService.GetById(account.BankId);
                return StatusCode(201, await _accountService.Create(account));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

   

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            var account = await _accountService.GetById(id);

            if (account == null) { return NotFound($"Account with id {id} not found"); }

            await _accountService.Delete(id);

            return Ok(await _accountService.GetAll());
        }

        private bool ContainsNullOrEmpty(Account account)
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
            if (string.IsNullOrEmpty(account.CustomerDocumentNumber))
            {
                _logger.LogError("AccountCustomerDocumentNumber is null or empty");
                return true;
            }
            if (account.BankId == Guid.Empty)
            {
                _logger.LogError("AccountBankId is empty");
                return true;
            }

            return false;
        }

        private bool AccountExists(Account account)
        {
            if (_dbContext.Account.Any(a => a.CustomerDocumentNumber == account.CustomerDocumentNumber))
            {
                _logger.LogError($"Account with CustomerDocumentNumber {account.CustomerDocumentNumber} already exists");
                return true;
            }
            if (_dbContext.Account.Any(a => a.Number == account.Number))
            {
                _logger.LogError($"Account with Number {account.Number} already exists");
                return true;
            }
            return false;
        }

        private bool ValidateBankAndCustomer(Account account)
        {
            var bank = _bankService.GetById(account.BankId);
            var customer = _customerService.GetById(account.CustomerDocumentNumber);
            if (bank == null)
            {
                _logger.LogError($"Bank with id {account.BankId} does not exist");
                return false;
            }
            if (customer == null)
            {
                _logger.LogError($"Customer with Document Number {account.CustomerDocumentNumber} does not exist");
                return false;
            }
            return true;
        }

    }
}
