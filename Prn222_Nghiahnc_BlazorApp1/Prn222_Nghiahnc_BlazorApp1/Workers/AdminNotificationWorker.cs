using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prn222_Nghiahnc_BlazorApp1.Hubs;
using Services;

namespace Prn222_Nghiahnc_BlazorApp1.Workers
{
    /// <summary>
    /// Background Worker that runs every 30 seconds to:
    /// 1. Count pending organizer registrations from DB
    /// 2. Count open transaction disputes from DB
    /// 3. If counts changed since last check → push SignalR notification to all admins
    ///
    /// Technique: IHostedService (BackgroundService) + SignalR integration.
    /// Architecture: Worker uses IServiceScopeFactory (Scoped services cannot be
    /// injected into Singleton-lifetime IHostedService directly).
    /// </summary>
    public class AdminNotificationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<AdminHub> _hubContext;
        private readonly ILogger<AdminNotificationWorker> _logger;

        private int _lastPendingOrganizers = -1;
        private int _lastOpenDisputes = -1;

        private static readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

        public AdminNotificationWorker(
            IServiceScopeFactory scopeFactory,
            IHubContext<AdminHub> hubContext,
            ILogger<AdminNotificationWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[AdminWorker] Started — checking every {Interval}s", _interval.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndNotifyAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[AdminWorker] Error during pending count check");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("[AdminWorker] Stopping.");
        }

        private async Task CheckAndNotifyAsync()
        {
            // Must create a scope because IUserService / ITransactionService are Scoped
            using var scope = _scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var txService = scope.ServiceProvider.GetRequiredService<ITransactionService>();

            int pendingOrganizers = userService.GetPendingOrganizerCount();
            int openDisputes = txService.GetOpenDisputeCount();

            bool changed = (pendingOrganizers != _lastPendingOrganizers)
                        || (openDisputes != _lastOpenDisputes);

            if (changed)
            {
                _logger.LogInformation(
                    "[AdminWorker] Count change detected — PendingOrganizers: {Prev}→{Now}, OpenDisputes: {PrevD}→{NowD}",
                    _lastPendingOrganizers, pendingOrganizers,
                    _lastOpenDisputes, openDisputes);

                _lastPendingOrganizers = pendingOrganizers;
                _lastOpenDisputes = openDisputes;

                // Broadcast to all connected admins via SignalR
                await AdminHub.BroadcastPendingCounts(_hubContext, pendingOrganizers, openDisputes);
            }
        }
    }
}
