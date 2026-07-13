using Domain;
using Microsoft.EntityFrameworkCore;
using MVC.Data2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrganizerAuditLogRepository : IOrganizerAuditLogRepository
    {
        private readonly DemoMVC2Context _context;

        public OrganizerAuditLogRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public async Task<List<OrganizerAuditLog>> GetByOrganizerIdAsync(int organizerId)
        {
            return await _context.OrganizerAuditLogs
                .Where(l => l.OrganizerId == organizerId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task CreateAsync(OrganizerAuditLog log)
        {
            _context.OrganizerAuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
