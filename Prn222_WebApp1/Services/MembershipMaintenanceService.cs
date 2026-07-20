using System.Data;
using Domain;
using Repositories;

namespace Services
{
    public class MembershipMaintenanceService : IMembershipMaintenanceService
    {
        private static readonly int[] PaidBookingStatuses =
        {
            CustomerWorkflowService.BookingStatusPaid,
            7
        };

        private readonly ICustomerRepository _repository;

        public MembershipMaintenanceService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<MembershipMaintenanceResult> RunExpiredMembershipMaintenanceAsync(DateTime asOf)
        {
            var userIds = await _repository.GetExpiredMembershipUserIdsAsync(asOf);
            var renewedCycles = 0;
            var downgradedCycles = 0;
            var skippedUsers = 0;

            foreach (var userId in userIds)
            {
                var result = await ProcessUserAsync(userId, asOf);
                renewedCycles += result.RenewedCycles;
                downgradedCycles += result.DowngradedCycles;
                if (result.Skipped)
                {
                    skippedUsers++;
                }
            }

            return new MembershipMaintenanceResult(
                userIds.Count,
                renewedCycles,
                downgradedCycles,
                skippedUsers);
        }

        private async Task<UserMaintenanceResult> ProcessUserAsync(int userId, DateTime asOf)
        {
            await using var dbTransaction = await _repository.BeginTransactionAsync(IsolationLevel.Serializable);
            var user = await _repository.GetUserByIdAsync(userId);

            if (user?.MembershipId == null
                || !user.MembershipExpiresAt.HasValue
                || user.MembershipExpiresAt.Value > asOf)
            {
                await dbTransaction.CommitAsync();
                return UserMaintenanceResult.SkippedResult;
            }

            var tiers = (await _repository.GetMembershipsAsync())
                .OrderBy(CustomerWorkflowService.GetTierThreshold)
                .ThenBy(m => m.Name)
                .ToList();
            var currentTierIndex = tiers.FindIndex(m => m.Id == user.MembershipId.Value);

            if (currentTierIndex < 0)
            {
                await dbTransaction.CommitAsync();
                return UserMaintenanceResult.SkippedResult;
            }

            var renewedCycles = 0;
            var downgradedCycles = 0;

            // Process missed yearly evaluations one period at a time if the app was offline.
            for (var cycle = 0; cycle < 20 && user.MembershipExpiresAt <= asOf; cycle++)
            {
                var currentTier = tiers[currentTierIndex];
                var periodEnd = user.MembershipExpiresAt!.Value;
                var periodStart = user.MembershipStartedAt ?? periodEnd.AddMonths(-12);
                if (periodStart >= periodEnd)
                {
                    periodStart = periodEnd.AddMonths(-12);
                }

                var paidTotal = await _repository.GetPaidTicketTotalAsync(
                    userId,
                    PaidBookingStatuses,
                    periodStart,
                    periodEnd);
                var maintenancePoints = CustomerWorkflowService.CalculatePoints(paidTotal);
                var requiredPoints = CustomerWorkflowService.GetTierThreshold(currentTier);
                var maintainsTier = maintenancePoints >= requiredPoints;

                var targetTierIndex = maintainsTier
                    ? currentTierIndex
                    : Math.Max(0, currentTierIndex - 1);
                var targetTier = tiers[targetTierIndex];
                var changeType = maintainsTier ? "Renewal" : "Downgrade";
                var reason = maintainsTier
                    ? $"Renewed with {maintenancePoints:N0}/{requiredPoints:N0} maintenance point(s) for {periodStart:d} - {periodEnd:d}."
                    : $"Downgraded one tier with {maintenancePoints:N0}/{requiredPoints:N0} maintenance point(s) for {periodStart:d} - {periodEnd:d}.";

                var nextStartedAt = periodEnd;
                DateTime? nextExpiresAt = CustomerWorkflowService.GetTierThreshold(targetTier) > 0
                    ? periodEnd.AddMonths(12)
                    : null;

                await _repository.UpdateUserMembershipAsync(
                    userId,
                    targetTier.Id,
                    nextStartedAt,
                    nextExpiresAt);
                await _repository.AddMembershipHistoryAsync(new MembershipHistory
                {
                    UserId = userId,
                    PreviousMembershipId = currentTier.Id,
                    NewMembershipId = targetTier.Id,
                    ChangedAt = asOf,
                    ChangeType = changeType,
                    Reason = reason
                });

                if (maintainsTier)
                {
                    renewedCycles++;
                }
                else
                {
                    downgradedCycles++;
                    currentTierIndex = targetTierIndex;
                }

                user.MembershipId = targetTier.Id;
                user.MembershipStartedAt = nextStartedAt;
                user.MembershipExpiresAt = nextExpiresAt;

                if (!nextExpiresAt.HasValue)
                {
                    break;
                }
            }

            await _repository.SaveChangesAsync();
            await dbTransaction.CommitAsync();
            return new UserMaintenanceResult(renewedCycles, downgradedCycles, false);
        }

        private record UserMaintenanceResult(int RenewedCycles, int DowngradedCycles, bool Skipped)
        {
            public static UserMaintenanceResult SkippedResult => new(0, 0, true);
        }
    }
}
