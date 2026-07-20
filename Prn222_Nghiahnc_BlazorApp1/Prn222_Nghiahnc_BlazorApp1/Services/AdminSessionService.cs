namespace Prn222_Nghiahnc_BlazorApp1.Services
{
    /// <summary>
    /// Scoped service (per Blazor Server circuit = per browser connection).
    /// Replaces HTTP Session for Blazor — stores admin UI preferences
    /// like filter state, page size, and last search across navigation.
    ///
    /// Technique: Session-equivalent pattern for Blazor Server.
    /// Registered as AddScoped so state persists within one browser tab session
    /// but resets when the tab closes or circuit disconnects.
    /// </summary>
    public class AdminSessionService
    {
        // ─── Account Management page ───────────────────────────────────────
        public string AccountSearchTerm { get; set; } = string.Empty;
        public int AccountFilterRole { get; set; } = -1;
        public int AccountFilterStatus { get; set; } = -1;
        public int AccountCurrentPage { get; set; } = 1;

        // ─── Approve Organizers page ───────────────────────────────────────
        public string OrganizerSearchTerm { get; set; } = string.Empty;
        public int OrganizerCurrentPage { get; set; } = 1;

        // ─── Transaction Disputes page ─────────────────────────────────────
        public string DisputeSearchTerm { get; set; } = string.Empty;
        public int DisputeCurrentPage { get; set; } = 1;

        // ─── System Settings page ──────────────────────────────────────────
        public string SettingsSearchTerm { get; set; } = string.Empty;
        public int SettingsCurrentPage { get; set; } = 1;

        // ─── Payment Gateway page ──────────────────────────────────────────
        public string GatewaySearchTerm { get; set; } = string.Empty;
        public int GatewayFilterActive { get; set; } = -1;
        public int GatewayCurrentPage { get; set; } = 1;

        // ─── Global preferences ────────────────────────────────────────────
        public int PageSize { get; set; } = 10;
        public string? AdminName { get; set; }
        public DateTime? LoginTime { get; set; }

        public void SetLoginInfo(string name)
        {
            AdminName = name;
            LoginTime = DateTime.Now;
        }

        public string GetSessionDuration()
        {
            if (LoginTime is null) return "—";
            var duration = DateTime.Now - LoginTime.Value;
            if (duration.TotalHours >= 1)
                return $"{(int)duration.TotalHours}h {duration.Minutes}m";
            return $"{duration.Minutes}m {duration.Seconds}s";
        }

        public void ResetAll()
        {
            AccountSearchTerm = string.Empty;
            AccountFilterRole = -1;
            AccountFilterStatus = -1;
            AccountCurrentPage = 1;
            OrganizerSearchTerm = string.Empty;
            OrganizerCurrentPage = 1;
            DisputeSearchTerm = string.Empty;
            DisputeCurrentPage = 1;
            SettingsSearchTerm = string.Empty;
            SettingsCurrentPage = 1;
            GatewaySearchTerm = string.Empty;
            GatewayFilterActive = -1;
            GatewayCurrentPage = 1;
        }
    }
}
