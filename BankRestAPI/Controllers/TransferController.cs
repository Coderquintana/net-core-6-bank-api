using BankRestAPI.Data;
using BankRestAPI.DTOs;
using BankRestAPI.Models;
using BankRestAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankRestAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]s")]
    public class TransferController : Controller
    {
        private readonly ILogger<TransferController> _logger;
        private readonly TransferService _transferService;
        private readonly BankService _bankService;
        private readonly AccountService _accountService;
        private readonly CustomerService _customerService;

        public TransferController(ILogger<TransferController> logger, TransferService transferService, BankService bankService, 
            AccountService accountService, CustomerService customerService)
        {
            _logger = logger;
            _transferService = transferService;
            _bankService = bankService;
            _accountService = accountService;
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransfers()
        {
            return Ok(await _transferService.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> Send(TransferDTO transferDto)
        {
            Transfer transfer = new Transfer();

            if (ContainsNullOrEmpty(transferDto))
            {
                return BadRequest("Required field cannot be null or empty");
            }

            var fromBank = await _bankService.GetByCode(transferDto.FromBank);
            var toBank = await _bankService.GetByCode(transferDto.ToBank);
            var fromAccount = await _accountService.GetByNumber(transferDto.FromAccount);
            var toAccount = await _accountService.GetByNumber(transferDto.ToAccount);
            var fromCustomer = await _customerService.GetById(transferDto.FromCustomer);
            var toCustomer = await _customerService.GetById(transferDto.ToCustomer);

            if (fromBank is null || toBank is null || fromAccount is null || toAccount is null
                || fromCustomer is null || toCustomer is null)
            {
                return BadRequest("Bank, Customer or Account not Found");

            }

            if (fromBank == toBank) 
            {
                return BadRequest("Both banks cannot be the same");
            }
            
            transfer.FromBank = fromBank;
            transfer.ToBank = toBank;
            transfer.FromAccount = fromAccount;
            transfer.ToAccount = toAccount;
            transfer.FromCustomer = fromCustomer;
            transfer.ToCustomer = toCustomer;

            transfer.Amount = transferDto.Amount;
            transfer.Currency = transferDto.Currency;
            transfer.State = "En proceso";
            transfer.OperationDate = DateTime.Now.ToUniversalTime();

            if (fromAccount.Balance < transfer.Amount)
            {
                return BadRequest("The account doesnt have that money");
            }

            toAccount.Balance = toAccount.Balance + transfer.Amount;
            fromAccount.Balance = fromAccount.Balance - transfer.Amount;

            await _accountService.Update(toAccount);
            await _accountService.Update(fromAccount);

            await _transferService.Create(transfer);
            return Ok(transfer);
        }

        private bool ContainsNullOrEmpty(TransferDTO transfer)
        {
            if(string.IsNullOrEmpty(transfer.Currency)) return true;
            if(string.IsNullOrEmpty(transfer.FromBank)) return true;
            if(string.IsNullOrEmpty(transfer.ToBank)) return true;
            if(string.IsNullOrEmpty(transfer.FromAccount)) return true;
            if(string.IsNullOrEmpty(transfer.ToAccount)) return true;
            if(string.IsNullOrEmpty(transfer.FromCustomer)) return true;
            if(string.IsNullOrEmpty(transfer.ToCustomer)) return true;

            return false;
        }

        [HttpPut("TransactionState/{id:guid}")]
        public async Task<IActionResult> UpdateTransactionState(Guid id, string transactionState)
        {
            var transfer = await _transferService.GetById(id);
            if (transfer == null)
            {
                return BadRequest("Transfer Not Found");
            }
            transfer.State = transactionState;
            await _transferService.Update(transfer);
            return Ok(transfer);
        }

        [HttpGet("TransactionState/{id:guid}")]
        public async Task<IActionResult> GetTransactionState(Guid id)
        {
            var transfer = await _transferService.GetById(id);
            if (transfer == null)
            {
                return NotFound("Transfer Not Found");
            }
            return Ok(transfer.State);
        }
    }
}
