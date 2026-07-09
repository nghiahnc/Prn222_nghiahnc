using System;

namespace Services
{
    public class UpdateEventRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EventCategoryId { get; set; }
        public string? Detail { get; set; }
        public string? Location { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int CurrentUserId { get; set; }
        public int CurrentUserRole { get; set; }
    }
}
