using Microsoft.Extensions.Options;
using Services;

namespace Prn222_MembershipWorker
{
    public class MembershipMaintenanceWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MembershipMaintenanceWorker> _logger;
        private readonly MembershipWorkerOptions _options;

        public MembershipMaintenanceWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<MembershipMaintenanceWorker> logger,
            IOptions<MembershipWorkerOptions> options)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_options.RunOnStartup)
            {
                await RunOnceAsync(stoppingToken);
            }

            var intervalHours = Math.Clamp(_options.IntervalHours, 1, 24 * 7);
            using var timer = new PeriodicTimer(TimeSpan.FromHours(intervalHours));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await RunOnceAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
        }

        private async Task RunOnceAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<IMembershipMaintenanceService>();
                var result = await service.RunExpiredMembershipMaintenanceAsync(DateTime.Now);

                _logger.LogInformation(
                    "Membership maintenance completed: {ExpiredUsers} expired user(s), {RenewedCycles} renewal(s), {DowngradedCycles} downgrade(s), {SkippedUsers} skipped.",
                    result.ExpiredUsers,
                    result.RenewedCycles,
                    result.DowngradedCycles,
                    result.SkippedUsers);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Membership maintenance failed and will retry on the next schedule.");
            }
        }
    }
}
