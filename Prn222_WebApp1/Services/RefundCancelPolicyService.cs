using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Repositories;

namespace Services
{
    public class RefundCancelPolicyService : IRefundCancelPolicyService
    {
        private readonly IRefundCancelPolicyRepository _policyRepo;
        private readonly IEventRepository _eventRepo;

        public RefundCancelPolicyService(
            IRefundCancelPolicyRepository policyRepo,
            IEventRepository eventRepo)
        {
            _policyRepo = policyRepo;
            _eventRepo = eventRepo;
        }

        public Task<List<RefundCancelPolicy>> GetAllAsync()
        {
            return _policyRepo.GetAllAsync();
        }

        public Task<RefundCancelPolicy?> GetByEventIdAsync(int eventId)
        {
            return _policyRepo.GetByEventIdAsync(eventId);
        }

        public async Task<ServiceResult> UpsertAsync(RefundCancelPolicyRequest request)
        {
            var eventEntity = await _eventRepo.GetByIdAsync(request.EventId);
            if (eventEntity == null)
                return ServiceResult.Fail("Event not found.");

            // Cannot create/edit/delete policy for any event whose TimeStart <= DateTime.Now
            if (eventEntity.TimeStart <= DateTime.Now)
                return ServiceResult.Fail("Cannot configure refund/cancel policy for an event that has already started.");

            if (request.RefundPercent < 0 || request.RefundPercent > 100)
                return ServiceResult.Fail("RefundPercent must be between 0 and 100.");
            if (request.RefundBeforeHours < 0)
                return ServiceResult.Fail("RefundBeforeHours cannot be negative.");
            if (request.CancelBeforeHours < 0)
                return ServiceResult.Fail("CancelBeforeHours cannot be negative.");

            // Normalize rules
            var refundBeforeHours = request.AllowRefund ? request.RefundBeforeHours : 0;
            var refundPercent = request.AllowRefund ? request.RefundPercent : 0;
            var cancelBeforeHours = request.AllowCancel ? request.CancelBeforeHours : 0;

            var existingPolicy = await _policyRepo.GetByEventIdAsync(request.EventId);

            if (existingPolicy == null)
            {
                var newPolicy = new RefundCancelPolicy
                {
                    EventId = request.EventId,
                    AllowRefund = request.AllowRefund,
                    RefundBeforeHours = refundBeforeHours,
                    RefundPercent = refundPercent,
                    AllowCancel = request.AllowCancel,
                    CancelBeforeHours = cancelBeforeHours,
                    PolicyNote = request.PolicyNote?.Trim()
                };
                await _policyRepo.AddAsync(newPolicy);
            }
            else
            {
                existingPolicy.AllowRefund = request.AllowRefund;
                existingPolicy.RefundBeforeHours = refundBeforeHours;
                existingPolicy.RefundPercent = refundPercent;
                existingPolicy.AllowCancel = request.AllowCancel;
                existingPolicy.CancelBeforeHours = cancelBeforeHours;
                existingPolicy.PolicyNote = request.PolicyNote?.Trim();
                _policyRepo.Update(existingPolicy);
            }

            await _policyRepo.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int eventId)
        {
            var eventEntity = await _eventRepo.GetByIdAsync(eventId);
            if (eventEntity == null)
                return ServiceResult.Fail("Event not found.");

            // Cannot create/edit/delete policy for any event whose TimeStart <= DateTime.Now
            if (eventEntity.TimeStart <= DateTime.Now)
                return ServiceResult.Fail("Cannot configure refund/cancel policy for an event that has already started.");

            var existingPolicy = await _policyRepo.GetByEventIdAsync(eventId);
            if (existingPolicy == null)
                return ServiceResult.Fail("Refund/cancel policy not found.");

            _policyRepo.Delete(existingPolicy);
            await _policyRepo.SaveChangesAsync();

            return ServiceResult.Ok();
        }
    }
}
