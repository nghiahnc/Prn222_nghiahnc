
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Ticket
    {
        public int Id { get; set; }

        public int TicketTypeId { get; set; }

        public string? QR { get; set; }

        public bool IsCheckedIn { get; set; }

        public DateTime? CheckInTime { get; set; }

        public int BookingId { get; set; }

        // Navigation
        public Booking? Booking { get; set; }

        public TicketType? TicketType { get; set; }
    }
}