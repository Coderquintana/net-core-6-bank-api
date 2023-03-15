using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankRestAPI.Models
{
    public class Bank 
    {
        [Key]
        public Guid Id { get; set; } = default;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
