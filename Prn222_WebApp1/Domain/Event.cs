using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Event
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int EventCategoryId { get; set; }

        public string? Detail { get; set; }

        public string? Location { get; set; }

        public DateTime TimeStart { get; set; }

        public DateTime TimeEnd { get; set; }

        public int Status { get; set; }

        public int? CreatedBy { get; set; }

        public User? Creator { get; set; }

        // Navigation Property
        public EventCategory? EventCategory { get; set; }
    }
}