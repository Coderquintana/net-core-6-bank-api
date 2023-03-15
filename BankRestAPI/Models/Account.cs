using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankRestAPI.Models
{
    public class Account
    {
        [JsonIgnore]
        public Guid Id { get; set; } = default!;
        public long? Number { get; set; } = default!;
        public string Currency { get; set; } = default!;
        public decimal Balance { get; set; } = default!;

        //Navigation property to Customer
        public string CustomerDocumentNumber { get; set; } = default!;
        [JsonIgnore]
        public Customer? Customer { get; set; }

        // Navigation properties to Bank
        public Guid BankId { get; set; } = default!;
        [JsonIgnore]
        public Bank? Bank { get; set; }

    }
}
