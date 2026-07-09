using System;
using System.Collections.Generic;
using Domain;

namespace Prn222_Nghiahnc_Mvc.Models
{
    public class EventFormViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EventCategoryId { get; set; }
        public string? Detail { get; set; }
        public string? Location { get; set; }
        public DateTime TimeStart { get; set; } = DateTime.Now.AddDays(1);
        public DateTime TimeEnd { get; set; } = DateTime.Now.AddDays(2);
        public List<EventCategory> Categories { get; set; } = new List<EventCategory>();
    }
}
