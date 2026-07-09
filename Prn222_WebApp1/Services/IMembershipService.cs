using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace Services
{
    public interface IMembershipService
    {
        Task<MembershipSummary?> GetMembershipSummaryAsync(int userId);
        Task<List<Membership>> GetAllAsync();
    }
}
