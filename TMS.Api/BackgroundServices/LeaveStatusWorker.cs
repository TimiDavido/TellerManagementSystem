using TMS.Application.Interfaces;

namespace TMS.Api.BackgroundServices
{
    public class LeaveStatusWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LeaveStatusWorker> _logger;

        public LeaveStatusWorker(IServiceScopeFactory scopeFactory, ILogger<LeaveStatusWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
            do
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<ILeaveStatusService>();
                    await service.ApplyLeaveStatusChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while applying leave status changes.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}
