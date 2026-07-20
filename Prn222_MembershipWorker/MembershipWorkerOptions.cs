namespace Prn222_MembershipWorker
{
    public class MembershipWorkerOptions
    {
        public const string SectionName = "MembershipMaintenance";

        public bool RunOnStartup { get; set; } = true;

        public int IntervalHours { get; set; } = 24;
    }
}
