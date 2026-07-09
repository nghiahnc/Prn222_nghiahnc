using System;

namespace Services
{
    public class TicketTypeRequest
    {
        public int? Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CurrentUserId { get; set; }
        public int CurrentUserRole { get; set; }
    }
}
