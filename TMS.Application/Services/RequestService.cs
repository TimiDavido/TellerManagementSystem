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
        private readonly IEmailService _emailService;

        public RequestService(IRequestRepository requestRepository, IUserRepository userRepository, IEmailService emailService)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<RequestDto> ApproveRequestAsync(Guid requestId, Guid approverId, string approverRole, ApproveRequestDto approveRequestDto)
        {
            var request = await _requestRepository.GetRequestsByIdAsync(requestId) ?? throw new KeyNotFoundException($"Request with ID {requestId} not found.");

            var isHOP = approverRole == "HOP";
            var isIC = approverRole == "IC";

            if (request.Status == RequestStatus.Pending && !isHOP)
            {
                throw new UnauthorizedAccessException("This request is waiting for HOP review.");
            }

            if (request.Status == RequestStatus.PendingIC && !isIC)
            {
                throw new UnauthorizedAccessException("This request is waiting for IC review.");
            }

            if (request.Status != RequestStatus.Pending && request.Status != RequestStatus.PendingIC)
            {
                throw new InvalidOperationException("This request has already been reviewed");
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

            request.Status = approveRequestDto.Decision == ApprovalDecision.Rejected ? RequestStatus.Rejected : request.Status == RequestStatus.Pending ? RequestStatus.PendingIC : RequestStatus.Approved;

            await _requestRepository.SaveReviewAsync(request, log);

            if (request.Status == RequestStatus.Approved || request.Status == RequestStatus.Rejected)
            {
                var subject = $"Your request has been {request.Status.ToString().ToLower()}";
                var body = $"Hello, \n\nYour {request.RequestType} request has been {request.Status.ToString().ToLower()}." + (string.IsNullOrWhiteSpace(approveRequestDto.Comment) ? "" : $"\n\nComment: {approveRequestDto.Comment}");

                var emailRequest = new SendSingleEmailRequest
                {
                    Receiver = request.Email,
                    Subject = subject,
                    Body = body
                };

                await _emailService.SendEmail(emailRequest);
            }

            return MapToDto(request);
        }

        public async Task<RequestDto> CreateRequestAsync(Guid userId, CreateRequestDto createRequestDto)
        {
            var user = await _userRepository.GetUserById(userId) ?? throw new KeyNotFoundException($"User not found.");

            var request = new Request
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Email = user.Email,
                RequestType = createRequestDto.RequestType,
                SubType = createRequestDto.SubType,
                Description = createRequestDto.Description,
                Status = RequestStatus.Pending,
                SubmittedAt = DateTime.UtcNow
            };

            await _requestRepository.AddRequestAsync(request);
            return MapToDto(request);
        }


        public async Task<List<RequestDto>> GetMyRequestsAsync(Guid userId)
        {
            var requests = await _requestRepository.GetRequestsByUserIdAsync(userId);
            return requests.Select(MapToDto).ToList();
        }

        public async Task<List<RequestDto>> GetPendingRequestsAsync(string role)
        {
            var status = role switch
            {
                "HOP" => RequestStatus.Pending,
                "IC" => RequestStatus.PendingIC,
                _ => throw new ArgumentException("You are not allowed to review requests.")
            };
            var requests = await _requestRepository.GetRequestsByStatusAsync(status);
            return requests.Select(MapToDto).ToList();
        }


        public async Task<List<RequestDto>> GetAllRequestsAsync(RequestStatus? status)
        {
            var requests = await _requestRepository.GetAllRequestsAsync(status);
            return requests.Select(MapToDto).ToList();
        }
        private static RequestDto MapToDto(Request request)
        {
            return new RequestDto
            {
                Id = request.Id,
                UserId = request.UserId,
                Email = request.Email,
                RequestType = request.RequestType,
                SubType = request.SubType,
                Status = request.Status.ToString(),
                SubmittedAt = request.SubmittedAt
            };
        }
    }
}
