using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Domain.Entities;
using TMS.Domain.Enums;

namespace TMS.Application.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;

        public RequestService(IRequestRepository requestRepository, IUserRepository userRepository)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
        }

        public async Task<RequestDto> ApproveRequestAsync(Guid requestId, Guid approverId, ApproveRequestDto approveRequestDto)
        {
            var request = await _requestRepository.GetRequestByIdAsync(requestId) ?? throw new KeyNotFoundException($"Request with ID {requestId} not found.");

            if (request.Status != RequestStatus.Pending)
            {
                throw new InvalidOperationException($"Request with ID {requestId} is not pending and cannot be approved.");
            }

            if (approveRequestDto.Decision == ApprovalDecision.Rejected && string.IsNullOrWhiteSpace(approveRequestDto.Comment))
                throw new ArgumentException("A comment is required when rejecting a request.");

            var approver = await _userRepository.GetUserById(approverId) ?? throw new KeyNotFoundException($"Approver with ID {approverId} not found.");

            var log = new RequestApprovalLog
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                ApproverId = approverId,
                RoleId = approver.RoleId,
                Decision = approveRequestDto.Decision,
                Comment = approveRequestDto.Comment,
                ReviewedAt = DateTime.UtcNow
            };

            request.Status = approveRequestDto.Decision == ApprovalDecision.Approved ? RequestStatus.Approved : RequestStatus.Rejected;

            await _requestRepository.SaveReviewAsync(request, log);
            return MapToDto(request);
        }

        public async Task<RequestDto> CreateRequestAsync(Guid userId, CreateRequestDto createRequestDto)
        {
            var request = new Request
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RequestType = createRequestDto.RequestType,
                SubType = createRequestDto.SubType,
                Status = RequestStatus.Pending,
                SubmittedAt = DateTime.UtcNow
            };

            await _requestRepository.AddRequestAsync(request);
            return MapToDto(request);
        }


        public async Task<List<RequestDto>> GetMyRequestsAsync(Guid userId)
        {
            var requests = await _requestRepository.GetRequestByUserIdAsync(userId);
            return requests.Select(MapToDto).ToList();
        }

        public async Task<List<RequestDto>> GetPendingRequestsAsync()
        {
            var requests = await _requestRepository.GetPendingRequestAsync();
            return requests.Select(MapToDto).ToList();
        }

        private static RequestDto MapToDto(Request request)
        {
            return new RequestDto
            {
                Id = request.Id,
                UserId = request.UserId,
                RequestType = request.RequestType,
                SubType = request.SubType,
                Status = request.Status.ToString(),
                SubmittedAt = request.SubmittedAt
            };
        }
    }
}
