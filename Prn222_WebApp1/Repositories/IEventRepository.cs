using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace Repositories
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(int id);
        Task<Event?> GetDetailsAsync(int id);
        Task AddAsync(Event entity);
        void Update(Event entity);
        void Delete(Event entity);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}
