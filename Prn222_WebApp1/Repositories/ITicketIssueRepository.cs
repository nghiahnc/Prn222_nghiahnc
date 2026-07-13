using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ITicketIssueRepository
    {
        Task<Booking?> GetBookingForTicketIssueAsync(int bookingId, int organizerId);

        Task<bool> HasGeneratedTicketsAsync(int bookingId);

        Task CreateTicketsAsync(List<Ticket> tickets);

        Task SaveChangesAsync();
    }
}
