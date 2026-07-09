using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;
using MVC.Data2;

namespace Repositories
{
    public class TicketTypeRepository : ITicketTypeRepository
    {
        private readonly DemoMVC2Context _context;

        public TicketTypeRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public Task<List<TicketType>> GetAllAsync()
        {
            return _context.TicketType
                .Include(tt => tt.Event)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<TicketType>> GetByEventIdAsync(int eventId)
        {
            return _context.TicketType
                .Where(tt => tt.EventId == eventId)
                .ToListAsync();
        }

        public Task<TicketType?> GetByIdAsync(int id)
        {
            return _context.TicketType
                .FirstOrDefaultAsync(tt => tt.Id == id);
        }

        public Task<TicketType?> GetDetailsAsync(int id)
        {
            return _context.TicketType
                .Include(tt => tt.Event)
                .FirstOrDefaultAsync(tt => tt.Id == id);
        }

        public async Task AddAsync(TicketType entity)
        {
            await _context.TicketType.AddAsync(entity);
        }

        public void Update(TicketType entity)
        {
            _context.TicketType.Update(entity);
        }

        public void Delete(TicketType entity)
        {
            _context.TicketType.Remove(entity);
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _context.TicketType.AnyAsync(tt => tt.Id == id);
        }

        public Task<bool> HasTicketTypesForEventAsync(int eventId)
        {
            return _context.TicketType.AnyAsync(tt => tt.EventId == eventId);
        }

        public Task<bool> HasSoldTicketsAsync(int ticketTypeId)
        {
            return _context.Ticket.AnyAsync(t => t.TicketTypeId == ticketTypeId);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
