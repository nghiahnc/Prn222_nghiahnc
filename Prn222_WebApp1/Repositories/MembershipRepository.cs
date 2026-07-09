using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;
using MVC.Data2;

namespace Repositories
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly DemoMVC2Context _context;

        public MembershipRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public Task<User?> GetUserWithMembershipAsync(int userId)
        {
            return _context.User
                .Include(u => u.Membership)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public Task<List<Membership>> GetAllAsync()
        {
            return _context.Membership
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<Membership?> GetByIdAsync(int id)
        {
            return _context.Membership
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
