namespace Services
{
    public interface IMembershipMaintenanceService
    {
        Task<MembershipMaintenanceResult> RunExpiredMembershipMaintenanceAsync(DateTime asOf);
    }

    public record MembershipMaintenanceResult(
        int ExpiredUsers,
        int RenewedCycles,
        int DowngradedCycles,
        int SkippedUsers);
}
