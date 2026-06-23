using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TicketType
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        // Navigation
        public Event? Event { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
            = new List<Ticket>();
    }
}