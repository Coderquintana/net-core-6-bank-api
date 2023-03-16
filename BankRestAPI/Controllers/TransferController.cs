﻿using BankRestAPI.Data;
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
            string result = string.Empty;

            result = ContainsNullOrEmpty(transferDto);
            if (!result.Equals("valid"))
            {
                return BadRequest("Required field cannot be null or empty");
            }

            var fromBank = await _bankService.GetByCode(transferDto.FromBank);
            var toBank = await _bankService.GetByCode(transferDto.ToBank);
            var fromAccount = await _accountService.GetByNumber(transferDto.FromAccount);
            var toAccount = await _accountService.GetByNumber(transferDto.ToAccount);
            var fromCustomer = await _customerService.GetById(transferDto.FromCustomer);
            var toCustomer = await _customerService.GetById(transferDto.ToCustomer);

            if (fromBank == toBank)
            {
                return BadRequest("Both banks can't be the same");
            }

            if (fromBank is null || toBank is null || fromAccount is null || toAccount is null
                || fromCustomer is null || toCustomer is null)
            {
                return NotFound("Bank, Customer or Account not Found");

            }

            if(fromAccount.Customer.DocumentNumber != fromCustomer.DocumentNumber)
            {
                return BadRequest("The FromAccount doesnt exists for that costumer");
            }
            if (toAccount.Customer.DocumentNumber != toCustomer.DocumentNumber)
            {
                return BadRequest("The ToAccount doesnt exists for that costumer");
            }
            if (fromAccount.Bank.Code != fromBank.Code)
            {
                return BadRequest("The FromAccount doesnt belong to that bank");
            }
            if (fromAccount.Bank.Code != toBank.Code)
            {
                return BadRequest("The ToAccount doesnt belong to that bank");
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
                return BadRequest("The account doesnt have enought money");
            }

            toAccount.Balance = toAccount.Balance + transfer.Amount;
            fromAccount.Balance = fromAccount.Balance - transfer.Amount;

            await _accountService.Update(toAccount);
            await _accountService.Update(fromAccount);

            await _transferService.Create(transfer);
            return Ok(transfer);
        }

        private string ContainsNullOrEmpty(TransferDTO transfer)
        {
            if(string.IsNullOrEmpty(transfer.Currency)) return "Currency required";
            if(string.IsNullOrEmpty(transfer.FromBank)) return "FromBank required";
            if(string.IsNullOrEmpty(transfer.ToBank)) return "ToBank required";
            if(string.IsNullOrEmpty(transfer.FromAccount)) return "FromAccount required";
            if(string.IsNullOrEmpty(transfer.ToAccount)) return "ToAccount required";
            if(string.IsNullOrEmpty(transfer.FromCustomer)) return "FromCustomer required";
            if(string.IsNullOrEmpty(transfer.ToCustomer)) return "ToCustomer required";

            return "valid";
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

        [HttpGet("Account/{accountNumber}")]
        public async Task<IActionResult> GetTransferByAccount(string accountNumber)
        {
            var transfer = await _transferService.FindByAccount(accountNumber);
            return Ok(transfer);
        }

        [HttpGet("Customer/{documentNumber}")]
        public async Task<IActionResult> GetTransferByCustomer(string documentNumber)
        {
            var transfer = await _transferService.FindByCustomer(documentNumber);
            return Ok(transfer);
        }
    }
}
