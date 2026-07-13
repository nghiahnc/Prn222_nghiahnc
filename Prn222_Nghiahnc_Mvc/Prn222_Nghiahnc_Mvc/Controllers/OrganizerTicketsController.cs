using Microsoft.AspNetCore.Mvc;
using Services;

namespace Prn222_Nghiahnc_Mvc.Controllers
{
    public class OrganizerTicketsController : Controller
    {
        private readonly ITicketIssueService _ticketIssueService;
        private readonly IOrganizerAuditLogService _auditLogService;

        public OrganizerTicketsController(
            ITicketIssueService ticketIssueService,
            IOrganizerAuditLogService auditLogService)
        {
            _ticketIssueService = ticketIssueService;
            _auditLogService = auditLogService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateTickets(int bookingId)
        {
            var organizerId = HttpContext.Session.GetInt32("UserId");

            if (organizerId == null)
            {
                return RedirectToAction("Login", "Logins");
            }

            var success = await _ticketIssueService.GenerateTicketsAsync(
                bookingId,
                organizerId.Value);

            if (!success)
            {
                TempData["Error"] = "Cannot generate tickets for this booking.";
                return RedirectToAction("Dashboard", "OrganizerReports");
            }

            await _auditLogService.AddLogAsync(
                organizerId.Value,
                "Generate QR Tickets",
                $"Organizer generated QR tickets for booking #{bookingId}.");

            TempData["Message"] = "Tickets generated successfully.";
            return RedirectToAction("Dashboard", "OrganizerReports");
        }
    }
}