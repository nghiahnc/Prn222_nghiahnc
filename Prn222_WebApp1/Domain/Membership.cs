using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Membership
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Point { get; set; }

        public string? Tier { get; set; }

        // Navigation
        public ICollection<User> Users { get; set; }
            = new List<User>();
    }
}