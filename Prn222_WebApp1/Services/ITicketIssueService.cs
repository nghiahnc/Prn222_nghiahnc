using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITicketIssueService
    {
        Task<bool> GenerateTicketsAsync(int bookingId, int organizerId);
    }
}
