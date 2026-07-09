using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace Repositories
{
    public interface ITicketTypeRepository
    {
        Task<List<TicketType>> GetAllAsync();
        Task<List<TicketType>> GetByEventIdAsync(int eventId);
        Task<TicketType?> GetByIdAsync(int id);
        Task<TicketType?> GetDetailsAsync(int id);
        Task AddAsync(TicketType entity);
        void Update(TicketType entity);
        void Delete(TicketType entity);
        Task<bool> ExistsAsync(int id);
        Task<bool> HasTicketTypesForEventAsync(int eventId);
        Task<bool> HasSoldTicketsAsync(int ticketTypeId);
        Task SaveChangesAsync();
    }
}
