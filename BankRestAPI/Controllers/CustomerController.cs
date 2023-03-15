using BankRestAPI.Data;
using BankRestAPI.Models;
using BankRestAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankRestAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]s")]
    public class CustomerController : Controller
    {
        private readonly CustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;


        public CustomerController(CustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            return Ok(await _customerService.GetAll());
        }

        [HttpGet("{documentNumber}")]
        public async Task<IActionResult> GetCustomer(string documentNumber)
        {
            var customer = await _customerService.GetById(documentNumber);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            try
            {
                if (ContainsNullOrEmpty(customer))
                {
                    return BadRequest("Required fields cannot be Empty or Null");
                }
                if (await CustomerExists(customer))
                {
                    return BadRequest("Customer already exists");
                }

                return StatusCode(201, await _customerService.Create(customer));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut("{documentNumber}")]
        public async Task<IActionResult> UpdateCustomer(string documentNumber, Customer customer)
        {
            _logger.LogInformation("Dentro de update");
            var entity = await _customerService.GetById(documentNumber);

            if (HasNullValue(documentNumber, customer))
            {
                return BadRequest();
            }
            Update(entity, customer);

            return Ok(entity);
        }


        [HttpDelete("{documentNumber}")]
        public async Task<IActionResult> DeleteCustomer(string documentNumber)
        {
            var customer = await _customerService.GetById(documentNumber);

            if (customer == null) { return NotFound($"Customer with Document Number {documentNumber} not found"); }

            await _customerService.Delete(documentNumber);

            return Ok(await _customerService.GetAll());
        }

        private async void Update(Customer entity, Customer customer)
        {
            if (!string.IsNullOrEmpty(customer.FullName))
            {
                entity.FullName = customer.FullName;
            }

            await _customerService.Update(entity);
        }

        private bool HasNullValue(string documentNumber, Customer customer)
        {
            if (customer == null)
            {
                _logger.LogError("Customer is null");
                return true;
            }
            if (documentNumber == null)
            {
                _logger.LogError("DocumentNumber is null");
                return true;
            }
            return false;
        }

        private bool ContainsNullOrEmpty(Customer customer)
        {
            if (customer == null)
            {
                _logger.LogInformation("Customer object is null or empty");
                return true;
            }
            if (string.IsNullOrEmpty(customer.DocumentNumber))
            {
                _logger.LogInformation("DocumentNumber is null or empty");
                return true;
            }
            if (string.IsNullOrEmpty(customer.DocumentType))
            {
                _logger.LogInformation("DocumentType is null or empty");
                return true;
            }
            if (string.IsNullOrEmpty(customer.FullName))
            {
                _logger.LogInformation("FullName is null or empty");
                return true;
            }
            return false;
        }

        private async Task<bool> CustomerExists(Customer customer)
        {
            var newCustomer = await _customerService.GetByDocument(customer.DocumentNumber);
            if (newCustomer != null)
                { return true; }
            
            return false;
        }
    }

}

