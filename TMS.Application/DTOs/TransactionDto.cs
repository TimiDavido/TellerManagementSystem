namespace TMS.Application.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public Guid TellerId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
    }
}
