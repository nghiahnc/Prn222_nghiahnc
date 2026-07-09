using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class RefundCancelPolicy
    {
        public int EventId { get; set; }

        public bool AllowRefund { get; set; }

        public int RefundBeforeHours { get; set; }

        public int RefundPercent { get; set; }

        public bool AllowCancel { get; set; }

        public int CancelBeforeHours { get; set; }

        public string? PolicyNote { get; set; }

        // Navigation Property
        public Event? Event { get; set; }
    }
}
