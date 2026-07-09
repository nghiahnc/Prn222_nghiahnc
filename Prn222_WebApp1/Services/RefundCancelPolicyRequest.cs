using System;

namespace Services
{
    public class RefundCancelPolicyRequest
    {
        public int EventId { get; set; }
        public bool AllowRefund { get; set; }
        public int RefundBeforeHours { get; set; }
        public int RefundPercent { get; set; }
        public bool AllowCancel { get; set; }
        public int CancelBeforeHours { get; set; }
        public string? PolicyNote { get; set; }
    }
}
