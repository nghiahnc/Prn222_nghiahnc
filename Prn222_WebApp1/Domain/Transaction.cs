using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Transaction
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? Detail { get; set; }

        public int Status { get; set; }

        public User? User { get; set; }
    }
}