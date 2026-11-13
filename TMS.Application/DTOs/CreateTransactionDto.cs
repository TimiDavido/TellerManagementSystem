using TMS.Domain.Enums;

namespace TMS.Application.DTOs
{
    public class CreateTransactionDto
    {
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string? Description { get; set; }
    }
}
