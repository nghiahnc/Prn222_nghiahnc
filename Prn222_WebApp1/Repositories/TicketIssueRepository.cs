using Domain;
using MVC.Data2;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class TicketIssueRepository : ITicketIssueRepository
    {
        private readonly DemoMVC2Context _context;

        public TicketIssueRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public async Task<Booking?> GetBookingForTicketIssueAsync(
            int bookingId,
            int organizerId)
        {
            return await _context.Booking
                .Include(b => b.User)
                .Include(b => b.Transaction)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.TicketType)
                        .ThenInclude(tt => tt.Event)
                .FirstOrDefaultAsync(b =>
                    b.Id == bookingId &&
                    b.Tickets.Any(t =>
                        t.TicketType!.Event!.CreatedBy == organizerId));
        }

        public async Task<bool> HasGeneratedTicketsAsync(int bookingId)
        {
            return await _context.Ticket
                .AnyAsync(t => t.BookingId == bookingId && !string.IsNullOrEmpty(t.QR));
        }

        public async Task CreateTicketsAsync(List<Ticket> tickets)
        {
            await _context.Ticket.AddRangeAsync(tickets);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
