namespace BankRestAPI.DTOs
{
    public class AccountDTO
    {
        public string Number { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; } = default;
        public string Customer { get; set; } = string.Empty;
        public string Bank { get; set; } = string.Empty;
    }

}
