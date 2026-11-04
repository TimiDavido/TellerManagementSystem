using TMS.Domain.Enums;

namespace TMS.Application.DTOs
{
    public class CreateTransactionDto
    {
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string? Description { get; set; }
    }
}
