using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class OrganizerAuditLog
    {
        public int Id { get; set; }

        public int OrganizerId { get; set; }

        public string Action { get; set; } = string.Empty;

        public string? Detail { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User? Organizer { get; set; }
    }
}
