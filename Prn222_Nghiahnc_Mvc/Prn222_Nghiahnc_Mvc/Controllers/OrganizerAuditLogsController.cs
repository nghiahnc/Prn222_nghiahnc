using Microsoft.AspNetCore.Mvc;
using Services;

namespace Prn222_Nghiahnc_Mvc.Controllers
{
    public class OrganizerAuditLogsController : Controller
    {
        private readonly IOrganizerAuditLogService _auditLogService;

        public OrganizerAuditLogsController(
            IOrganizerAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsOrganizer())
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var organizerId = GetCurrentUserId();

            if (organizerId == null)
            {
                return RedirectToAction("Login", "Logins");
            }

            await _auditLogService.AddLogAsync(
                organizerId.Value,
                "View Audit Logs",
                "Organizer viewed audit logs.");

            var logs = await _auditLogService.GetLogsAsync(organizerId.Value);

            return View(logs);
        }

        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        private bool IsOrganizer()
        {
            return HttpContext.Session.GetInt32("Role") == 2;
        }
    }
}