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
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; } 
    }
}
