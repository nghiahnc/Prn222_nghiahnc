using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;
using MVC.Data2;

namespace Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly DemoMVC2Context _context;

        public EventRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public Task<List<Event>> GetAllAsync()
        {
            return _context.Event
                .Include(e => e.EventCategory)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<Event?> GetByIdAsync(int id)
        {
            return _context.Event
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public Task<Event?> GetDetailsAsync(int id)
        {
            return _context.Event
                .Include(e => e.EventCategory)
                .Include(e => e.TicketTypes)
                .Include(e => e.RefundCancelPolicy)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(Event entity)
        {
            await _context.Event.AddAsync(entity);
        }

        public void Update(Event entity)
        {
            _context.Event.Update(entity);
        }

        public void Delete(Event entity)
        {
            _context.Event.Remove(entity);
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _context.Event.AnyAsync(e => e.Id == id);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
