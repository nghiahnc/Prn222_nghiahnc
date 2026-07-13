using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Booking
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public int TransactionId { get; set; }

        // xac nhan booking
        public bool ConfirmationSent { get; set; }

        public DateTime? ConfirmationSentAt { get; set; }

        // nhac nho event
        public bool ReminderSent { get; set; }

        public DateTime? ReminderSentAt { get; set; }

        public User? User { get; set; }

        public Transaction? Transaction { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
            = new List<Ticket>();
    }
}