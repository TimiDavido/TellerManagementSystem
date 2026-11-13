using TMS.Domain.Enums;

namespace TMS.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid TellerId { get; set; } 
        public decimal Amount { get; set; }
        public User Teller { get; set; } = null!;
        public TransactionType TransactionType { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; } 
    }
}
