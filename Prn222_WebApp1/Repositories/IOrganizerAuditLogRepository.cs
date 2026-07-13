using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IOrganizerAuditLogRepository
    {
        Task<List<OrganizerAuditLog>> GetByOrganizerIdAsync(int organizerId);

        Task CreateAsync(OrganizerAuditLog log);
    }
}
