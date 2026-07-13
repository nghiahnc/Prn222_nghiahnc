using Domain;
using Microsoft.EntityFrameworkCore;
using MVC.Data2;


namespace Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DemoMVC2Context _context;

        public BookingRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public async Task CreateAsync(Booking booking)
        {
            await _context.Booking.AddAsync(booking);
        }

        public async Task<Booking?> GetBookingWithUserAsync(int bookingId)
        {
            return await _context.Booking
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task<List<Booking>> GetBookingsNeedReminderAsync(DateTime reminderTime)
        {
            return await _context.Booking
                .Include(b => b.User)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.TicketType)
                        .ThenInclude(tt => tt.Event)
                .Where(b =>
                    !b.ReminderSent &&
                    b.Tickets.Any(t =>
                        t.TicketType!.Event!.TimeStart <= reminderTime &&
                        t.TicketType.Event.TimeStart > DateTime.Now))
                .ToListAsync();
        }

        public async Task MarkConfirmationSentAsync(int bookingId)
        {
            var booking = await _context.Booking.FindAsync(bookingId);

            if (booking == null)
            {
                return;
            }

            booking.ConfirmationSent = true;
            booking.ConfirmationSentAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task MarkReminderSentAsync(int bookingId)
        {
            var booking = await _context.Booking.FindAsync(bookingId);

            if (booking == null)
            {
                return;
            }

            booking.ReminderSent = true;
            booking.ReminderSentAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
