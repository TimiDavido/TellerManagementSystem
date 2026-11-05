using TMS.Application.DTOs;

namespace TMS.Application.Interfaces
{
    public interface IRequestService
    {
        Task<RequestDto> CreateRequestAsync(Guid userId, CreateRequestDto createRequestDto);
        Task<List<RequestDto>> GetMyRequestsAsync(Guid userId);
        Task<List<RequestDto>> GetPendingRequestsAsync();
        Task<RequestDto> ApproveRequestAsync(Guid requestId, Guid approverId, ApproveRequestDto approveRequestDto);
    }
}
