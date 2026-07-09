using System;

namespace Services
{
    public class PostponeEventRequest
    {
        public int EventId { get; set; }
        public DateTime NewTimeStart { get; set; }
        public DateTime NewTimeEnd { get; set; }
        public string? Reason { get; set; }
        public int CurrentUserId { get; set; }
        public int CurrentUserRole { get; set; }
    }
}
