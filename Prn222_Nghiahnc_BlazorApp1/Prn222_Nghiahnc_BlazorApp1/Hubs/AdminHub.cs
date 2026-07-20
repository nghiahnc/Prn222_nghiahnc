using Microsoft.AspNetCore.SignalR;

namespace Prn222_Nghiahnc_BlazorApp1.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time admin notifications.
    ///
    /// Clients join "admins" group when they connect.
    /// The server broadcasts events to all admins simultaneously —
    /// e.g., when organizer is approved, all open admin tabs see the
    /// updated badge count instantly without page reload.
    ///
    /// Technique: SignalR for real-time push notifications.
    /// </summary>
    public class AdminHub : Hub
    {
        private const string AdminGroup = "admins";

        public override async Task OnConnectedAsync()
        {
            // All clients that connect to this hub join the admins group
            await Groups.AddToGroupAsync(Context.ConnectionId, AdminGroup);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, AdminGroup);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>Called by server to broadcast organizer approved event.</summary>
        public static async Task NotifyOrganizerApproved(
            IHubContext<AdminHub> hubContext,
            string username,
            int pendingCount)
        {
            await hubContext.Clients.Group(AdminGroup)
                .SendAsync("OrganizerApproved", username, pendingCount);
        }

        /// <summary>Called by server to broadcast organizer rejected event.</summary>
        public static async Task NotifyOrganizerRejected(
            IHubContext<AdminHub> hubContext,
            string username,
            int pendingCount)
        {
            await hubContext.Clients.Group(AdminGroup)
                .SendAsync("OrganizerRejected", username, pendingCount);
        }

        /// <summary>Called by server to broadcast account banned event.</summary>
        public static async Task NotifyAccountBanned(
            IHubContext<AdminHub> hubContext,
            string username)
        {
            await hubContext.Clients.Group(AdminGroup)
                .SendAsync("AccountBanned", username);
        }

        /// <summary>Called by server to broadcast account reactivated event.</summary>
        public static async Task NotifyAccountActivated(
            IHubContext<AdminHub> hubContext,
            string username)
        {
            await hubContext.Clients.Group(AdminGroup)
                .SendAsync("AccountActivated", username);
        }

        /// <summary>Called by server to broadcast dispute resolved.</summary>
        public static async Task NotifyDisputeResolved(
            IHubContext<AdminHub> hubContext,
            int transactionId,
            bool wasRefunded,
            int openDisputeCount)
        {
            await hubContext.Clients.Group(AdminGroup)
                .SendAsync("DisputeResolved", transactionId, wasRefunded, openDisputeCount);
        }

        /// <summary>
        /// Broadcast updated pending counts to all admins.
        /// Called by Background Worker every 30 seconds.
        /// </summary>
        public static async Task BroadcastPendingCounts(
            IHubContext<AdminHub> hubContext,
            int pendingOrganizers,
            int openDisputes)
        {
            await hubContext.Clients.Group(AdminGroup)
                .SendAsync("PendingCountsUpdated", pendingOrganizers, openDisputes);
        }
    }
}
