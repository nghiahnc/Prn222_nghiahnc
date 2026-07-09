using System;

namespace Services
{
    public class MembershipSummary
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Tier { get; set; }
        public int Points { get; set; }
        public string? Benefits { get; set; }
        public int? NextTierPoints { get; set; }
        public int? PointsToNextTier { get; set; }
    }
}
