using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Services;
using Prn222_Nghiahnc_Mvc.Models;

namespace Prn222_Nghiahnc_Mvc.Controllers
{
    public class RefundCancelPoliciesController : Controller
    {
        private readonly IRefundCancelPolicyService _policyService;
        private readonly IEventService _eventService;

        public RefundCancelPoliciesController(
            IRefundCancelPolicyService policyService,
            IEventService eventService)
        {
            _policyService = policyService;
            _eventService = eventService;
        }

        private int CurrentUserId => HttpContext.Session.GetInt32("UserId") ?? 0;
        private int CurrentUserRole => HttpContext.Session.GetInt32("Role") ?? -1;

        private async Task<bool> CanManageEventAsync(int eventId)
        {
            var role = CurrentUserRole;
            if (role == 0) return true;
            if (role == 2)
            {
                var ev = await _eventService.GetDetailsAsync(eventId);
                if (ev == null) return false;
                
                // Block if Cancelled or Started
                if (ev.Status == 2) return false;
                if (ev.TimeStart <= DateTime.Now) return false;

                return ev.CreatedBy == CurrentUserId;
            }
            return false;
        }

        // GET: /RefundCancelPolicies
        public async Task<IActionResult> Index()
        {
            if (CurrentUserRole != 0)
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied: Admin role required.");

            var policies = await _policyService.GetAllAsync();
            return View(policies);
        }

        // GET: /RefundCancelPolicies/Manage?eventId=5
        public async Task<IActionResult> Manage(int eventId)
        {
            if (!await CanManageEventAsync(eventId))
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");

            var eventEntity = await _eventService.GetDetailsAsync(eventId);
            if (eventEntity == null)
                return NotFound();

            var policy = await _policyService.GetByEventIdAsync(eventId);

            var vm = new RefundCancelPolicyViewModel
            {
                EventId = eventId,
                Event = eventEntity,
                AllowRefund = policy?.AllowRefund ?? false,
                RefundBeforeHours = policy?.RefundBeforeHours ?? 0,
                RefundPercent = policy?.RefundPercent ?? 0,
                AllowCancel = policy?.AllowCancel ?? false,
                CancelBeforeHours = policy?.CancelBeforeHours ?? 0,
                PolicyNote = policy?.PolicyNote
            };

            return View(vm);
        }

        // POST: /RefundCancelPolicies/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(RefundCancelPolicyViewModel vm)
        {
            if (!await CanManageEventAsync(vm.EventId))
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");

            var request = new RefundCancelPolicyRequest
            {
                EventId = vm.EventId,
                AllowRefund = vm.AllowRefund,
                RefundBeforeHours = vm.RefundBeforeHours,
                RefundPercent = vm.RefundPercent,
                AllowCancel = vm.AllowCancel,
                CancelBeforeHours = vm.CancelBeforeHours,
                PolicyNote = vm.PolicyNote
            };

            var result = await _policyService.UpsertAsync(request);
            if (result.Success)
            {
                TempData["Success"] = "Policy updated successfully.";
                return RedirectToAction("Details", "Events", new { id = vm.EventId });
            }

            ModelState.AddModelError("", result.Error ?? "Error occurred during policy update.");
            vm.Event = await _eventService.GetDetailsAsync(vm.EventId);
            return View(vm);
        }

        // POST: /RefundCancelPolicies/Delete?eventId=5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int eventId)
        {
            if (!await CanManageEventAsync(eventId))
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");

            var result = await _policyService.DeleteAsync(eventId);
            if (result.Success)
            {
                TempData["Success"] = "Policy deleted successfully.";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Error occurred during policy deletion.";
            }

            return RedirectToAction("Details", "Events", new { id = eventId });
        }
    }
}
