using System;

namespace Prn222_Nghiahnc_Mvc.Models
{
    public class PostponeEventViewModel
    {
        public int EventId { get; set; }
        public DateTime CurrentTimeStart { get; set; }
        public DateTime CurrentTimeEnd { get; set; }
        public DateTime NewTimeStart { get; set; }
        public DateTime NewTimeEnd { get; set; }
        public string? Reason { get; set; }
    }
}
