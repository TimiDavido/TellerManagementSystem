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

            if (request.Status == RequestStatus.Approved && request.LeaveStart is not null)
            {
                request.LeaveProgress = LeaveProgress.Scheduled;
            }

            if (request.Status == RequestStatus.Rejected && request.LeaveStart is not null)
            {
                request.LeaveProgress = LeaveProgress.Rejected;
            }

            if (!Enum.IsDefined(typeof(ApprovalDecision), approveRequestDto.Decision))
                throw new ArgumentOutOfRangeException("Decision must be 'approved' or 'rejected'.");

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

            if (request.Status == RequestStatus.Approved && request.LeaveStart is not null)
                request.LeaveProgress = LeaveProgress.Scheduled;

            if (request.Status == RequestStatus.Rejected && request.LeaveStart is not null)
                request.LeaveProgress = LeaveProgress.Rejected;

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

            var isLeave = string.Equals(createRequestDto.RequestType, "Leave", StringComparison.OrdinalIgnoreCase);

            var request = new Request
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Email = user.Email,
                RequestType = createRequestDto.RequestType,
                SubType = createRequestDto.SubType,
                Description = createRequestDto.Description,
                Status = RequestStatus.Pending,
                SubmittedAt = DateTime.UtcNow,
                LeaveStart = isLeave ? createRequestDto.LeaveStart : null,
                LeaveEnd = isLeave ? createRequestDto.LeaveEnd : null,
            };

            var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(1));

            if (isLeave)
            {
                if (createRequestDto.LeaveStart is null || createRequestDto.LeaveEnd is null)
                    throw new ArgumentException("A leave request must have both a start and end date.");

                if (createRequestDto.LeaveStart < today)
                    throw new ArgumentException("Leave start date cannot be in the past.");

                if (createRequestDto.LeaveEnd < createRequestDto.LeaveStart)
                    throw new ArgumentException("Leave end date cannot be before the start date.");
            }

            if (!isLeave)
            {
                if (createRequestDto.LeaveStart is not null || createRequestDto.LeaveEnd is not null)
                    throw new ArgumentException("Dates can only be added for a leave request!");
            }

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
                _ => throw new UnauthorizedAccessException("You are not allowed to review requests.")
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
                Description = request.Description,
                Status = request.Status.ToString(),
                SubmittedAt = request.SubmittedAt,
                LeaveStart = request.LeaveStart,
                LeaveEnd = request.LeaveEnd,
            };
        }
    }
}
