using System.Text.Json.Serialization;
using TMS.Domain.Enums;

namespace TMS.Application.DTOs
{
    public class ApproveRequestDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApprovalDecision Decision { get; set; }
        public string? Comment { get; set; } 
    }
}
