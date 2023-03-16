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
        // Service Injection, logger and constructors
        private readonly ILogger<BankController> _logger;
        private readonly BankService _bankService;

        public BankController(ILogger<BankController> logger,
            BankService bankService)
        {
            _logger = logger;
            _bankService = bankService;
        }

        // api/v1/Banks ------------------- GET -------------------
        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            return Ok(await _bankService.GetAll());
        }

        // api/v1/Banks/{id} ------------------- GET BY CODE -------------------
        [HttpGet("{code}")]
        public async Task<IActionResult> GetBank(string code)
        {
            var bank = await _bankService.GetByCode(code);

            if (bank == null)
            {
                return NotFound("Bank not Found");
            }

            return Ok(bank);
        }

        // api/v1/Banks ------------------- POST -------------------
        [HttpPost]
        public async Task<IActionResult> AddBank(BankDTO bankDto)
        {
            try
            {
                Bank bank = new Bank();
                string result = string.Empty;

                result = ContainsNullOrEmpty(bankDto);
                if (!result.Equals("valid"))
                {
                    return BadRequest(result);
                }

                result = await BankUpdateIsValid(bankDto);
                if (!result.Equals("valid"))
                {
                    return BadRequest(result);
                }

                bank.Name = bankDto.Name;
                bank.Address = bankDto.Address;
                bank.Code = bankDto.Code;

                await _bankService.Create(bank);
                return StatusCode(201, bank);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // api/v1/Banks/{code} ------------------- PUT -------------------
        [HttpPut]
        [Route("{code}")]
        public async Task<IActionResult> UpdateBank(string code, BankDTO bankDto)
        {
            var entity = await _bankService.GetByCode(code);
            string result = string.Empty;

            if (entity == null)
            {
                _logger.LogError($"Bank {entity} doesnt exists");
                return NotFound("Bank not Found");
            }

            result = await BankUpdateIsValid(bankDto);
            if (!result.Equals("valid"))
            {
                return BadRequest(result);
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

        // api/v1/Banks ------------------- DELETE -------------------
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteBank(string code)
        {
            var bank = await _bankService.GetByCode(code);

            if (bank == null) { return NotFound($"Bank with id {code} not found"); }

            await _bankService.Delete(bank.Id);

            return Ok(await _bankService.GetAll());
        }

        // ------------------- My New Bank Null or Empty Validations -------------------
        private string ContainsNullOrEmpty(BankDTO bank)
        {
            if (bank == null)
            {
                _logger.LogError("Bank object is null");
                return "Bank object is null";
            }
            if (string.IsNullOrEmpty(bank.Name))
            {
                _logger.LogError("BankName is null or empty");
                return "Bank Name is null or empty";
            }
            if (string.IsNullOrEmpty(bank.Address))
            {
                _logger.LogError("BankAddress is null or empty");
                return "Bank Address is null or empty";
            }

            return "valid";
        }

        // ------------------- My Banks Validations -------------------
        private async Task<string> BankUpdateIsValid(BankDTO bank)
        {
            var bankCode = await _bankService.GetByCode(bank.Code);
            var bankName = await _bankService.GetByName(bank.Name);
            if (bankCode != null)
            {
                _logger.LogError($"Bank {bank.Code} already exists");
                return $"Bank {bank.Code} already exists";
            }

            if (bankName != null)
            {
                _logger.LogError($"Bank {bank.Name} already exists");
                return $"Bank {bank.Name} already exists";
            }

            return "valid";
        }

    }
}
