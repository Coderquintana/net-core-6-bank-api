using BankRestAPI.Data;
using BankRestAPI.Models;
using BankRestAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankRestAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]s")]
    public class BankController : Controller
    {
        private readonly ILogger<BankController> _logger;
        private readonly IEntityService<Bank> _bankService;
        private readonly BankDbContext _dbContext;

        public BankController(ILogger<BankController> logger,
            IEntityService<Bank> bankService, BankDbContext dbContext)
        {
            _logger = logger;
            _bankService = bankService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            return Ok(await _bankService.GetAll());
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBank(Guid id)
        {
            var bank = await _bankService.GetById(id);

            if (bank == null)
            {
                return NotFound();
            }

            return Ok(bank);
        }


        [HttpPost]
        public async Task<IActionResult> AddBank(Bank bank)
        {
            try
            {
                if (ContainsNullOrEmpty(bank) || BankExists(bank) ) 
                { 
                    return BadRequest(); 
                }
                
                return StatusCode(201, await _bankService.Create(bank));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateBank(Guid id, Bank bank)
        {

            var entity = await _bankService.GetById(id);

            if (entity == null)
            {
                _logger.LogError($"Bank {entity} is null");
                return BadRequest();
            }

            if (BankExists(bank))
            {
                return BadRequest();
            }

            Update(entity, bank);

            return Ok(entity);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBank(Guid id)
        {
            var bank = await _bankService.GetById(id);

            if (bank == null) { return NotFound($"Bank with id {id} not found"); }

            await _bankService.Delete(id);

            return Ok(await _bankService.GetAll());
        }

        private async void Update(Bank entity, Bank bank)
        {
            if (!string.IsNullOrEmpty(bank.Name))
            {
                entity.Name = bank.Name;
            }
            if (!string.IsNullOrEmpty(bank.Address))
            {
                entity.Address = bank.Address;
            }
            await _bankService.Update(entity);
        }

        private bool ContainsNullOrEmpty(Bank bank)
        {
            if (bank == null)
            {
                _logger.LogError("Bank object is null");
                return true;
            }
            if (string.IsNullOrEmpty(bank.Name))
            {
                _logger.LogError("BankName is null or empty");
                return true;
            }
            if (string.IsNullOrEmpty(bank.Address))
            {
                _logger.LogError("BankAddress is null or empty");
                return true;
            }

            return false;
        }

        private bool BankExists(Bank bank)
        {
            if (_dbContext.Bank.Any(b => b.Name == bank.Name && b.Id != bank.Id))
            {
                _logger.LogError($"Bank {bank.Name} already exists");
                return true;
            }
            if (_dbContext.Bank.Any(b => b.Address == bank.Address && b.Id != bank.Id))
            {
                _logger.LogError($"Address {bank.Address} corresponds to another Bank");
                return true;
            }
            return false;
        }

    }
}
