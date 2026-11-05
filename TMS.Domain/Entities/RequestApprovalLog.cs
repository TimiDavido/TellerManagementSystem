using TMS.Domain.Enums;

namespace TMS.Domain.Entities
{
    public class RequestApprovalLog
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public Request Request { get; set; } = null!;
        public Guid ApproverId { get; set; }
        public User Approver { get; set; } = null!;
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public ApprovalDecision Decision { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow; 
    }
}
