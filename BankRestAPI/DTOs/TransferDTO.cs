namespace BankRestAPI.DTOs
{
    public class TransferDTO
    {
        public string FromBankName { get; set; } = string.Empty;
        public string FromCustomer { get; set; } = string.Empty;
        public string FromBank { get; set; } = string.Empty;
        public string FromAccount { get; set; } = string.Empty;

        public string ToBankName { get; set; } = string.Empty;
        public string ToCustomer { get; set; } = string.Empty;
        public string ToBank { get; set; } = string.Empty;
        public string ToAccount { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;
        public decimal Amount { get; set; } = default;
    }
}
