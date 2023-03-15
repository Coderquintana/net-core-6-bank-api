using BankRestAPI.Data;
using BankRestAPI.DTOs;
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
        private readonly BankService _bankService;

        public BankController(ILogger<BankController> logger,
            BankService bankService)
        {
            _logger = logger;
            _bankService = bankService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            return Ok(await _bankService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBank(Guid id)
        {
            var bank = await _bankService.GetById(id);

            if (bank == null)
            {
                return NotFound("Bank not Found");
            }

            return Ok(bank);
        }


        [HttpPost]
        public async Task<IActionResult> AddBank(BankDTO bankDto)
        {
            try
            {
                Bank bank = new Bank();

                if (ContainsNullOrEmpty(bankDto))
                {
                    return BadRequest("Field required cannot be null or empty");
                }
                if (await BankExists(bankDto))
                {
                    return BadRequest("Bank already exists");
                }

                bank.Name = bankDto.Name;
                bank.Address = bankDto.Address;
                bank.Code = bankDto.Code;

                await _bankService.Create(bank);
                return Ok(bank);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("{code}")]
        public async Task<IActionResult> UpdateBank(string code, BankDTO bankDto)
        {
            var entity = await _bankService.GetByCode(code);

            if (entity == null)
            {
                _logger.LogError($"Bank {entity} is null");
                return BadRequest("Bank not Found");
            }

            if (await BankExists(bankDto))
            {
                return BadRequest("Bank Already Exists");
            }

            if (!string.IsNullOrEmpty(bankDto.Code))
            {
                entity.Code = bankDto.Code;
            }
            if (!string.IsNullOrEmpty(bankDto.Name))
            {
                entity.Name = bankDto.Name;
            }
            if (!string.IsNullOrEmpty(bankDto.Address))
            {
                entity.Address = bankDto.Address;
            }
            await _bankService.Update(entity);

            return Ok(entity);
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteBank(string code)
        {
            var bank = await _bankService.GetByCode(code);

            if (bank == null) { return NotFound($"Bank with id {code} not found"); }

            await _bankService.Delete(bank.Id);

            return Ok(await _bankService.GetAll());
        }

        private bool ContainsNullOrEmpty(BankDTO bank)
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

        private async Task<bool> BankExists(BankDTO bank)
        {
            var bankCode = await _bankService.GetByCode(bank.Code);
            var bankName = await _bankService.GetByName(bank.Name);
            if (bankCode != null)
            {
                _logger.LogError($"Bank {bank.Code} already exists");
                return true;
            }

            if (bankName != null)
            {
                _logger.LogError($"Bank {bank.Name} already exists");
                return true;
            }

            return false;
        }

    }
}
