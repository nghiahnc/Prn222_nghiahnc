using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;
using MVC.Data2;

namespace Repositories
{
    public class EventCategoryRepository : IEventCategoryRepository
    {
        private readonly DemoMVC2Context _context;

        public EventCategoryRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public Task<List<EventCategory>> GetAllAsync()
        {
            return _context.EventCategorie
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _context.EventCategorie.AnyAsync(ec => ec.Id == id);
        }
    }
}
