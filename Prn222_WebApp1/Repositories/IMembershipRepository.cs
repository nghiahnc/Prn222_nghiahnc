using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace Repositories
{
    public interface IMembershipRepository
    {
        Task<User?> GetUserWithMembershipAsync(int userId);
        Task<List<Membership>> GetAllAsync();
        Task<Membership?> GetByIdAsync(int id);
    }
}
