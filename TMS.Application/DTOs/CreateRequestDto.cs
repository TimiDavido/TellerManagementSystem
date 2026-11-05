namespace TMS.Application.DTOs
{
    public class CreateRequestDto
    {
        public string RequestType { get; set; } = string.Empty;
        public string? SubType { get; set; }
        public string? Description { get; set; }
    }
}
