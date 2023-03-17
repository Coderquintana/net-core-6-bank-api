using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankRestAPI.Models
{
    public class Transfer
    {
        [Key]
        public Guid Id { get; set; } = default;

        public string FromBankName { get; set; } = string.Empty;
        public Customer FromCustomer { get; set; } = new Customer();
        public Bank FromBank { get; set; } = new Bank();
        public Account FromAccount { get; set; } = new Account();

        public string ToBankName { get; set; } = string.Empty;
        public Customer ToCustomer { get; set; } = new Customer();
        public Bank ToBank { get; set; } = new Bank();
        public Account ToAccount { get; set; } = new Account();

        public DateTime OperationDate { get; set; } = default!;
        public string Currency { get; set; } = string.Empty;
        public decimal Amount { get; set; } = default;
        public string State { get; set; } = string.Empty;

    }
}
