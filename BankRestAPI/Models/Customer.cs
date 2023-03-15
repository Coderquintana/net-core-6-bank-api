using System.ComponentModel.DataAnnotations;

namespace BankRestAPI.Models
{
    public class Customer
    {
        [Key]
        public string DocumentNumber { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
