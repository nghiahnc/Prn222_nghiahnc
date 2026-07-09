using System;

namespace Services
{
    public class CreateEventRequest
    {
        public string Name { get; set; } = string.Empty;
        public int EventCategoryId { get; set; }
        public string? Detail { get; set; }
        public string? Location { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int CreatedBy { get; set; }
    }
}
