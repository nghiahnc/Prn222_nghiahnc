using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace Repositories
{
    public interface IRefundCancelPolicyRepository
    {
        Task<List<RefundCancelPolicy>> GetAllAsync();
        Task<RefundCancelPolicy?> GetByEventIdAsync(int eventId);
        Task<RefundCancelPolicy?> GetDetailsByEventIdAsync(int eventId);
        Task AddAsync(RefundCancelPolicy entity);
        void Update(RefundCancelPolicy entity);
        void Delete(RefundCancelPolicy entity);
        Task<bool> ExistsForEventAsync(int eventId);
        Task SaveChangesAsync();
    }
}
