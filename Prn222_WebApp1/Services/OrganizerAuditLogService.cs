using Domain;
using Repositories;

namespace Services
{
    public class OrganizerAuditLogService : IOrganizerAuditLogService
    {
        private readonly IOrganizerAuditLogRepository _repository;

        public OrganizerAuditLogService(IOrganizerAuditLogRepository repository)
        {
            _repository = repository;
        }

        public Task<List<OrganizerAuditLog>> GetLogsAsync(int organizerId)
        {
            return _repository.GetByOrganizerIdAsync(organizerId);
        }

        public async Task AddLogAsync(
            int organizerId,
            string action,
            string? detail)
        {
            var log = new OrganizerAuditLog
            {
                OrganizerId = organizerId,
                Action = action,
                Detail = detail,
                CreatedAt = DateTime.Now
            };

            await _repository.CreateAsync(log);
        }
    }
}
