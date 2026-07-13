using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TicketSaleStatistic
    {
        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public int TotalTicketsSold { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}
