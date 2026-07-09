using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace Services
{
    public interface IRefundCancelPolicyService
    {
        Task<List<RefundCancelPolicy>> GetAllAsync();
        Task<RefundCancelPolicy?> GetByEventIdAsync(int eventId);
        Task<ServiceResult> UpsertAsync(RefundCancelPolicyRequest request);
        Task<ServiceResult> DeleteAsync(int eventId);
    }
}
