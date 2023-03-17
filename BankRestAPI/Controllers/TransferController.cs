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
        // Service Injection, logger and constructors
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

        // api/v1/Transfers ------------------- GET -------------------
        [HttpGet]
        public async Task<IActionResult> GetTransfers()
        {
            return Ok(await _transferService.GetAll());
        }

        // api/v1/Transfers ------------------- POST -------------------
        [HttpPost]
        public async Task<IActionResult> Send(TransferDTO transferDto)
        {
            Transfer transfer = new Transfer();
            string result = string.Empty;

            result = ContainsNullOrEmpty(transferDto);
            if (!result.Equals("valid"))
            {
                return BadRequest(result);
            }

            result = await TransferIsValid(transferDto);
            if (!result.Equals("valid"))
            {
                return BadRequest(result);
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

            transfer.FromBankName = fromBank.Name;
            transfer.ToBankName = toBank.Name;
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
                return BadRequest("The account doesnt have enought money");
            }

            toAccount.Balance = toAccount.Balance + transfer.Amount;
            fromAccount.Balance = fromAccount.Balance - transfer.Amount;

            await _accountService.Update(toAccount);
            await _accountService.Update(fromAccount);

            await _transferService.Create(transfer);
            return Ok(transfer);
        }

        // api/v1/Transfers/State/{id} ------------------- PUT STATE BY ID -------------------
        [HttpPut("State/{id:guid}")]
        public async Task<IActionResult> UpdateTransactionState(Guid id, string transactionState)
        {
            var transfer = await _transferService.GetById(id);
            if (transfer == null)
            {
                return NotFound("Transfer Not Found");
            }
            transfer.State = transactionState;
            await _transferService.Update(transfer);
            return Ok(transfer);
        }

        // api/v1/Transfer/State/{id} ------------------- GET STATE BY ID -------------------
        [HttpGet("State/{id:guid}")]
        public async Task<IActionResult> GetTransactionState(Guid id)
        {
            var transfer = await _transferService.GetById(id);
            if (transfer == null)
            {
                return NotFound("Transfer Not Found");
            }
            return Ok(transfer.State);
        }

        // api/v1/Transfer/Account/{accountNumber} ------------------- GET TRANSFERS BY ACCOUNT NUMBER -------------------
        [HttpGet("Account/{accountNumber}")]
        public async Task<IActionResult> GetTransferByAccount(string accountNumber)
        {
            var transfer = await _transferService.FindByAccount(accountNumber);
            if (transfer is null)
            {
                return NotFound();
            }
            return Ok(transfer);
        }

        // api/v1/Transfer/Customer/{documentNumber} ------------------- GET TRANSFERS BY DOCUMENT NUMBER -------------------
        [HttpGet("Customer/{documentNumber}")]
        public async Task<IActionResult> GetTransferByCustomer(string documentNumber)
        {
            var transfer = await _transferService.FindByCustomer(documentNumber);
            if (transfer is null)
            {
                return NotFound();
            }
            return Ok(transfer);
        }

        // ------------------- My New Transfer Null or Empty Validations -------------------
        private string ContainsNullOrEmpty(TransferDTO transfer)
        {
            if (string.IsNullOrEmpty(transfer.Currency)) return "Currency required";
            if (string.IsNullOrEmpty(transfer.FromBank)) return "FromBank required";
            if (string.IsNullOrEmpty(transfer.ToBank)) return "ToBank required";
            if (string.IsNullOrEmpty(transfer.FromAccount)) return "FromAccount required";
            if (string.IsNullOrEmpty(transfer.ToAccount)) return "ToAccount required";
            if (string.IsNullOrEmpty(transfer.FromCustomer)) return "FromCustomer required";
            if (string.IsNullOrEmpty(transfer.ToCustomer)) return "ToCustomer required";

            return "valid";
        }

        // ------------------- My Transfers Validations -------------------
        private async Task<string> TransferIsValid(TransferDTO transferDto)
        {

            var fromBank = await _bankService.GetByCode(transferDto.FromBank);
            var toBank = await _bankService.GetByCode(transferDto.ToBank);
            var fromAccount = await _accountService.GetByNumber(transferDto.FromAccount);
            var toAccount = await _accountService.GetByNumber(transferDto.ToAccount);
            var fromCustomer = await _customerService.GetById(transferDto.FromCustomer);
            var toCustomer = await _customerService.GetById(transferDto.ToCustomer);

            if (fromBank == toBank)
            {
                return "Both banks can't be the same";
            }

            if (fromBank is null || toBank is null || fromAccount is null || toAccount is null
                || fromCustomer is null || toCustomer is null)
            {
                return "Bank, Customer or Account not Found";

            }

            if (fromAccount.Customer.DocumentNumber != fromCustomer.DocumentNumber)
            {
                return "The FromAccount doesnt exists for that costumer";
            }
            if (toAccount.Customer.DocumentNumber != toCustomer.DocumentNumber)
            {
                return "The ToAccount doesnt exists for that costumer";
            }
            if (fromAccount.Bank.Code != fromBank.Code)
            {
                return "The FromAccount doesnt belong to that bank";
            }
            if (toAccount.Bank.Code != toBank.Code)
            {
                return "The ToAccount doesnt belong to that bank";
            }
            if (transferDto.FromBankName != fromBank.Name)
            {
                return "The fromBank name doesnt match with the Bank code";
            }
            if (transferDto.ToBankName != toBank.Name)
            {
                return "The toBank name doesnt match with the Bank code";
            }

            return "valid";
        }
    }
}
