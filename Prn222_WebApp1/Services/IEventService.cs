using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace Services
{
    public interface IEventService
    {
        Task<List<Event>> GetAllAsync();
        Task<Event?> GetDetailsAsync(int id);
        Task<List<EventCategory>> GetCategoriesAsync();
        Task<ServiceResult<int>> CreateAsync(CreateEventRequest request);
        Task<ServiceResult> UpdateAsync(UpdateEventRequest request);
        Task<ServiceResult> DeleteAsync(int id, int currentUserId, int currentUserRole);
        Task<ServiceResult> PublishAsync(int id, int currentUserId, int currentUserRole);
        Task<ServiceResult> CancelAsync(int id, int currentUserId, int currentUserRole);
        Task<ServiceResult> PostponeAsync(PostponeEventRequest request);
    }
}
