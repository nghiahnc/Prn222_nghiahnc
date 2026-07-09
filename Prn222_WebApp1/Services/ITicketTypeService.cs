using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace Services
{
    public interface ITicketTypeService
    {
        Task<List<TicketType>> GetAllAsync();
        Task<TicketType?> GetDetailsAsync(int id);
        Task<List<TicketType>> GetByEventIdAsync(int eventId);
        Task<ServiceResult<int>> CreateAsync(TicketTypeRequest request);
        Task<ServiceResult> UpdateAsync(TicketTypeRequest request);
        Task<ServiceResult> DeleteAsync(int id, int currentUserId, int currentUserRole);
    }
}
