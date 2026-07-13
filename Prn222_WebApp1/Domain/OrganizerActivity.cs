using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class OrganizerActivity
    {
        public int TotalEvents { get; set; }

        public int TotalBookings { get; set; }

        public int TotalTicketsSold { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}
