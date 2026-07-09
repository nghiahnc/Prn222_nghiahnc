using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace Repositories
{
    public interface IEventCategoryRepository
    {
        Task<List<EventCategory>> GetAllAsync();
        Task<bool> ExistsAsync(int id);
    }
}
