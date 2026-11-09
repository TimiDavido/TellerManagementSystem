using TMS.Application.DTOs;
using TMS.Domain.Enums;

namespace TMS.Application.Interfaces
{
    public interface IRequestService
    {
        Task<RequestDto> CreateRequestAsync(Guid userId, CreateRequestDto createRequestDto);
        Task<List<RequestDto>> GetAllRequestsAsync(RequestStatus? status);
        Task<List<RequestDto>> GetMyRequestsAsync(Guid userId);
        Task<List<RequestDto>> GetPendingRequestsAsync(string role);
        Task<RequestDto> ApproveRequestAsync(Guid requestId, Guid approverId, string approverRole, ApproveRequestDto approveRequestDto);
    }
}
