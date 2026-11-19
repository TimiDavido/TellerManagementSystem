using TMS.Domain.Entities;
using TMS.Domain.Enums;

namespace TMS.Application.Interfaces
{
    public interface IRequestRepository
    {
        Task<Request> AddRequestAsync(Request request);
        Task<List<Request>> GetAllRequestsAsync(RequestStatus? status = null);
        Task<Request?> GetRequestsByIdAsync(Guid requestId);
        Task<List<Request>> GetRequestsByUserIdAsync(Guid userId);
        Task<List<Request>> GetPendingRequestAsync();
        Task<List<Request>> GetRequestsByStatusAsync(RequestStatus status);
        Task<List<Request>> GetLeavesByProgressAsync(LeaveProgress leaveProgress);
        Task UpdateRequestAsync(Request request);
        Task SaveReviewAsync(Request request, RequestApprovalLog log);
    }
}
