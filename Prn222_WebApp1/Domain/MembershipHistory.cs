namespace Domain
{
    public class MembershipHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int? PreviousMembershipId { get; set; }

        public int NewMembershipId { get; set; }

        public DateTime ChangedAt { get; set; }

        public string ChangeType { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public User? User { get; set; }

        public Membership? PreviousMembership { get; set; }

        public Membership? NewMembership { get; set; }
    }
}
