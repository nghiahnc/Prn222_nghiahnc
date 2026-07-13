
namespace Domain
{
    public class OrganizerReport
    {

        public int TotalEvents { get; set; }

        public int TotalBookings { get; set; }

        public int TotalTicketsSold { get; set; }

        public decimal TotalRevenue { get; set; }

        public List<TicketSaleStatistic> TicketSales { get; set; } = new();

        public List<EventRevenue> EventRevenues { get; set; } = new();
    }
}
