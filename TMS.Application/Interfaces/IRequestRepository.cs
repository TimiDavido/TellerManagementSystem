using TMS.Domain.Entities;

namespace TMS.Application.Interfaces
{
    public interface IRequestRepository
    {
        Task<Request> AddRequestAsync(Request request);
        Task<Request?> GetRequestByIdAsync(Guid requestId);
        Task<List<Request>> GetRequestByUserIdAsync(Guid userId);
        Task<List<Request>> GetPendingRequestAsync();
        Task SaveReviewAsync(Request request, RequestApprovalLog log);
    }
}
