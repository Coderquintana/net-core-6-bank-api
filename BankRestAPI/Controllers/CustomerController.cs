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
        // Service Injection, logger and constructors
        private readonly CustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(CustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        // api/v1/Customers ------------------- GET -------------------
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            return Ok(await _customerService.GetAll());
        }

        // api/v1/Customers/{documentNumber} ------------------- GET BY Document Number -------------------
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

        // api/v1/Customers ------------------- POST -------------------
        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            try
            {
                string result = string.Empty;

                result = ContainsNullOrEmpty(customer);
                if (!result.Equals("valid"))
                {
                    return BadRequest(result);
                }

                result = await CustomerExists(customer);
                if (!result.Equals("valid"))
                {
                    return BadRequest(result);
                }

                var entity = await _customerService.Create(customer);
                return StatusCode(201, entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        // api/v1/Customers/{documentNumber} ------------------- PUT -------------------
        [HttpPut("{documentNumber}")]
        public async Task<IActionResult> UpdateCustomer(string documentNumber, Customer customer)
        {
            var entity = await _customerService.GetById(documentNumber);
            string result = string.Empty;

            if (entity == null)
            {
                return NotFound("Customer Not Found");
            }

            result = await CustomerExists(customer);
            if (!result.Equals("valid"))
            {
                return BadRequest(result);
            }

            if (!string.IsNullOrEmpty(customer.FullName))
            {
                entity.FullName = customer.FullName;
            }
            if (!string.IsNullOrEmpty(customer.DocumentNumber))
            {
                entity.DocumentNumber = customer.DocumentNumber;
            }

            await _customerService.Update(entity);  
            return Ok(entity);
        }

        // api/v1/Customers ------------------- DELETE -------------------
        [HttpDelete("{documentNumber}")]
        public async Task<IActionResult> DeleteCustomer(string documentNumber)
        {
            var customer = await _customerService.GetById(documentNumber);

            if (customer == null) { return NotFound($"Customer with Document Number {documentNumber} not found"); }

            await _customerService.Delete(documentNumber);

            return Ok(await _customerService.GetAll());
        }

        // ------------------- My New Customer Null or Empty Validations -------------------
        private string ContainsNullOrEmpty(Customer customer)
        {
            if (customer == null)
            {
                _logger.LogInformation("Customer object is null or empty");
                return "Customer object is null or empty";
            }
            if (string.IsNullOrEmpty(customer.DocumentNumber))
            {
                _logger.LogInformation("DocumentNumber is null or empty");
                return "DocumentNumber is null or empty";
            }
            if (string.IsNullOrEmpty(customer.DocumentType))
            {
                _logger.LogInformation("DocumentType is null or empty");
                return "DocumentType is null or empty";
            }
            if (string.IsNullOrEmpty(customer.FullName))
            {
                _logger.LogInformation("FullName is null or empty");
                return "FullName is null or empty";
            }
            return "valid";
        }

        // ------------------- My Customer Validations -------------------
        private async Task<string> CustomerExists(Customer customer)
        {
            var newCustomer = await _customerService.GetByDocument(customer.DocumentNumber);
            if (newCustomer != null)
                { return "That document Number Belong to an existent Customer";}
            
            return "valid";
        }
    }

}

