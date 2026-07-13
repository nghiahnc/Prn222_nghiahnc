using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    internal class SystemAuditLog
    {
        public int Id { get; set; }

        public string Action { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Detail { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
