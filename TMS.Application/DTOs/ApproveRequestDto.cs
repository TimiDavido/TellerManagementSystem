using TMS.Domain.Enums;

namespace TMS.Application.DTOs
{
    public class ApproveRequestDto
    {
        public ApprovalDecision Decision { get; set; }
        public string? Comment { get; set; } 
    }
}
