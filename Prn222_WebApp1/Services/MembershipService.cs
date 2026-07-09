using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Repositories;

namespace Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepo;

        public MembershipService(IMembershipRepository membershipRepo)
        {
            _membershipRepo = membershipRepo;
        }

        public async Task<MembershipSummary?> GetMembershipSummaryAsync(int userId)
        {
            var user = await _membershipRepo.GetUserWithMembershipAsync(userId);
            if (user == null)
                return null;

            var points = user.Membership?.Point ?? 0;
            var tier = user.Membership?.Tier ?? "Bronze";

            // Determine benefits
            string benefits = "Normal access";
            int? nextTierPoints = 500;

            var normalizedTier = tier.Trim().ToLowerInvariant();
            if (normalizedTier == "silver")
            {
                benefits = "5% discount";
                nextTierPoints = 2000;
            }
            else if (normalizedTier == "gold")
            {
                benefits = "10% discount";
                nextTierPoints = 5000;
            }
            else if (normalizedTier == "platinum")
            {
                benefits = "15% discount and VIP perks";
                nextTierPoints = null;
            }
            else
            {
                // Default to Bronze
                benefits = "Normal access";
                nextTierPoints = 500;
            }

            int? pointsToNextTier = null;
            if (nextTierPoints.HasValue)
            {
                pointsToNextTier = Math.Max(0, nextTierPoints.Value - points);
            }

            return new MembershipSummary
            {
                UserId = user.Id,
                UserName = user.UserName,
                Tier = tier,
                Points = points,
                Benefits = benefits,
                NextTierPoints = nextTierPoints,
                PointsToNextTier = pointsToNextTier
            };
        }

        public Task<List<Membership>> GetAllAsync()
        {
            return _membershipRepo.GetAllAsync();
        }
    }
}
