using BankRestAPI.Data;
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
        private readonly BankDbContext _bankDbContext;
        private readonly IEntityService<Transfer> _transferService;
        private readonly IEntityService<Bank> _bankService;

        public TransferController(ILogger<TransferController> logger, BankDbContext bankDbContext, IEntityService<Transfer> transferService, IEntityService<Bank> bankService)
        {
            _logger = logger;
            _bankDbContext = bankDbContext;
            _transferService = transferService;
            _bankService = bankService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransfers()
        {
            return Ok(await _transferService.GetAll());
        }

        [HttpPost("Send")]
        public async Task<IActionResult> Send(Transfer transfer)
        {
            if (ContainsNullOrEmpty(transfer))
            {
                return BadRequest();
            }

            var fromBank = await _bankService.GetById(transfer.FromBankId);
            var toBank = await _bankService.GetById(transfer.ToBankId);

            //if (fromBank == null || toBank == null)
            //{
            //    _logger.LogError("Al menos uno de los bancos no existe");
            //    return BadRequest();
            //}
            if(fromBank == toBank) 
            {
                throw new Exception(message: "Bancos NO deben ser Iguales");
            }
            return StatusCode(100);
        }

        private bool ContainsNullOrEmpty(Transfer transfer)
        {
            throw new NotImplementedException();
        }

        [HttpPost("Receive")]
        public async Task<IActionResult> Receive(Transfer transfer)
        {
            throw new NotImplementedException();
        }

        [HttpPut("TransactionState/{id:guid}")]
        public async Task<IActionResult> UpdateTransactionState(Guid id, string transactionState)
        {
            throw new NotImplementedException();
        }

        [HttpGet("TransactionState/{id:guid}")]
        public async Task<IActionResult> GetTransactionState(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
