using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class QRCodeService : IQRCodeService
    {
        public string GenerateTicketCode(int bookingId, int ticketTypeId, int index)
        {
            return $"BOOKING-{bookingId}-TYPE-{ticketTypeId}-TICKET-{index}-{Guid.NewGuid()}";
        }
    }
}
