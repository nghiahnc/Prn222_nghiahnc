using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IBookingRepository
    {
        Task CreateAsync(Booking booking);

        Task<Booking?> GetBookingWithUserAsync(int bookingId);

        Task<List<Booking>> GetBookingsNeedReminderAsync(DateTime reminderTime);

        Task MarkConfirmationSentAsync(int bookingId);

        Task MarkReminderSentAsync(int bookingId);

        Task SaveChangesAsync();
    }
}
