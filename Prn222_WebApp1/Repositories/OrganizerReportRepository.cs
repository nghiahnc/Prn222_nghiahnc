using MVC.Data2;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class OrganizerReportRepository : IOrganizerReportRepository
    {
        private readonly DemoMVC2Context _context;

        public OrganizerReportRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public async Task<OrganizerActivity> GetOrganizerActivityAsync(int organizerId)
        {
            var totalEvents = await _context.Event
                .CountAsync(e => e.CreatedBy == organizerId);

            var totalBookings = await _context.Booking
                .CountAsync(b => b.Tickets.Any(t =>
                    t.TicketType!.Event!.CreatedBy == organizerId));

            var totalTicketsSold = await _context.Ticket
                .CountAsync(t => t.TicketType!.Event!.CreatedBy == organizerId);

            var totalRevenue = await _context.Ticket
                .Where(t => t.TicketType!.Event!.CreatedBy == organizerId)
                .SumAsync(t => (decimal?)t.TicketType!.Price) ?? 0;

            return new OrganizerActivity
            {
                TotalEvents = totalEvents,
                TotalBookings = totalBookings,
                TotalTicketsSold = totalTicketsSold,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<List<TicketSaleStatistic>> GetTicketSalesStatisticsAsync(int organizerId)
        {
            return await _context.Ticket
                .Where(t => t.TicketType!.Event!.CreatedBy == organizerId)
                .GroupBy(t => new
                {
                    t.TicketType!.Event!.Id,
                    t.TicketType.Event.Name
                })
                .Select(g => new TicketSaleStatistic
                {
                    EventId = g.Key.Id,
                    EventName = g.Key.Name,
                    TotalTicketsSold = g.Count(),
                    TotalRevenue = g.Sum(t => t.TicketType!.Price)
                })
                .ToListAsync();
        }

        public async Task<List<EventRevenue>> GetEventRevenueAsync(int organizerId)
        {
            return await _context.Ticket
                .Where(t => t.TicketType!.Event!.CreatedBy == organizerId)
                .GroupBy(t => new
                {
                    t.TicketType!.Event!.Id,
                    t.TicketType.Event.Name
                })
                .Select(g => new EventRevenue
                {
                    EventId = g.Key.Id,
                    EventName = g.Key.Name,
                    Revenue = g.Sum(t => t.TicketType!.Price)
                })
                .ToListAsync();
        }

        public async Task<OrganizerReport> GetOrganizerReportAsync(int organizerId)
        {
            var activity = await GetOrganizerActivityAsync(organizerId);
            var ticketSales = await GetTicketSalesStatisticsAsync(organizerId);
            var eventRevenues = await GetEventRevenueAsync(organizerId);

            return new OrganizerReport
            {
                TotalEvents = activity.TotalEvents,
                TotalBookings = activity.TotalBookings,
                TotalTicketsSold = activity.TotalTicketsSold,
                TotalRevenue = activity.TotalRevenue,
                TicketSales = ticketSales,
                EventRevenues = eventRevenues
            };
        }
    }
}
