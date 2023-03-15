using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankRestAPI.Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; } = new Guid();
        public string Number { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; } = default!;
        public Customer Customer { get; set; } = new Customer();
        public Bank Bank { get; set; } = new Bank();

    }
}
