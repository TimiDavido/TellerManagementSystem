using TMS.Domain.Enums;

namespace TMS.Domain.Entities
{
    public class Request
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Email { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string? SubType { get; set; }
        public string? Description { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public ICollection<RequestApprovalLog> ApprovalLogs { get; set; } = new List<RequestApprovalLog>();
    }
}
