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

        // Navigation properties
        public User? User { get; set; }

        public Transaction? Transaction { get; set; }
    }
}