using TMS.Application.Interfaces;
using TMS.Domain.Enums;

namespace TMS.Application.Services
{
    public class LeaveStatusService : ILeaveStatusService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;

        public LeaveStatusService(IRequestRepository requestRepository, IUserRepository userRepository)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
        }

        public async Task ApplyLeaveStatusChangesAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(1));

            foreach (var leave in await _requestRepository.GetLeavesByProgressAsync(LeaveProgress.Scheduled))
            {
                if (leave.LeaveEnd < today)
                {
                    leave.LeaveProgress = LeaveProgress.Completed;
                }
                else if  (leave.LeaveStart <= today)
                {
                    var user = await _userRepository.GetUserById(leave.UserId);
                    if (user is not null && user.AccountStatus == "ACTIVE")
                    {
                        user.AccountStatus = "INACTIVE";
                        await _userRepository.UpdateUser(user);
                        leave.AccountInactive = true;
                    }
                    leave.LeaveProgress = LeaveProgress.OnLeave;
                }
                await _requestRepository.UpdateRequestAsync(leave);
            }

            foreach (var leave in await _requestRepository.GetLeavesByProgressAsync(LeaveProgress.OnLeave))
            {
                if (leave.LeaveEnd <= today) continue;

                if (leave.AccountInactive)
                {
                    var user = await _userRepository.GetUserById(leave.UserId);
                    if (user is not null && user.AccountStatus == "INACTIVE")
                    {
                        user.AccountStatus = "ACTIVE";
                        await _userRepository.UpdateUser(user);
                    }
                    leave.LeaveProgress = LeaveProgress.Completed;
                    await _requestRepository.UpdateRequestAsync(leave);
                }
            }
        }
    }
}
