using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IOrganizerAuditLogService
    {
        Task<List<OrganizerAuditLog>> GetLogsAsync(int organizerId);

        Task AddLogAsync(int organizerId, string action, string? detail);
    }
}
