using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;
using MVC.Data2;

namespace Repositories
{
    public class RefundCancelPolicyRepository : IRefundCancelPolicyRepository
    {
        private readonly DemoMVC2Context _context;

        public RefundCancelPolicyRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public Task<List<RefundCancelPolicy>> GetAllAsync()
        {
            return _context.RefundCancelPolicy
                .Include(r => r.Event)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<RefundCancelPolicy?> GetByEventIdAsync(int eventId)
        {
            return _context.RefundCancelPolicy
                .FirstOrDefaultAsync(r => r.EventId == eventId);
        }

        public Task<RefundCancelPolicy?> GetDetailsByEventIdAsync(int eventId)
        {
            return _context.RefundCancelPolicy
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.EventId == eventId);
        }

        public async Task AddAsync(RefundCancelPolicy entity)
        {
            await _context.RefundCancelPolicy.AddAsync(entity);
        }

        public void Update(RefundCancelPolicy entity)
        {
            _context.RefundCancelPolicy.Update(entity);
        }

        public void Delete(RefundCancelPolicy entity)
        {
            _context.RefundCancelPolicy.Remove(entity);
        }

        public Task<bool> ExistsForEventAsync(int eventId)
        {
            return _context.RefundCancelPolicy.AnyAsync(r => r.EventId == eventId);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
