using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text;

namespace Prn222_Nghiahnc_Mvc.Controllers
{
    public class OrganizerReportsController : Controller
    {
        private readonly IOrganizerReportService _reportService;
        private readonly IOrganizerAuditLogService _auditLogService;

        public OrganizerReportsController(
            IOrganizerReportService reportService,
            IOrganizerAuditLogService auditLogService)
        {
            _reportService = reportService;
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var organizerId = GetCurrentUserId();

            if (organizerId == null)
            {
                return RedirectToAction("Login", "Logins");
            }

            var data = await _reportService
                .GetOrganizerActivityAsync(organizerId.Value);

            return View(data);
        }

        public async Task<IActionResult> TicketSales()
        {
            var organizerId = GetCurrentUserId();

            if (organizerId == null)
            {
                return RedirectToAction("Login", "Logins");
            }

            await _auditLogService.AddLogAsync(
                organizerId.Value,
                "View Ticket Sales",
                "Organizer viewed ticket sales statistics.");

            var data = await _reportService
                .GetTicketSalesStatisticsAsync(organizerId.Value);

            return View(data);
        }

        public async Task<IActionResult> EventRevenue()
        {
            var organizerId = GetCurrentUserId();

            if (organizerId == null)
            {
                return RedirectToAction("Login", "Logins");
            }

            await _auditLogService.AddLogAsync(
                organizerId.Value,
                "View Event Revenue",
                "Organizer viewed event revenue report.");

            var data = await _reportService
                .GetEventRevenueAsync(organizerId.Value);

            return View(data);
        }

        public async Task<IActionResult> ExportReport()
        {
            var organizerId = GetCurrentUserId();

            if (organizerId == null)
            {
                return RedirectToAction("Login", "Logins");
            }

            await _auditLogService.AddLogAsync(
                organizerId.Value,
                "Export Report",
                "Organizer exported CSV report.");

            var report = await _reportService
                .GetOrganizerReportAsync(organizerId.Value);

            var csv = new StringBuilder();

            csv.AppendLine("Organizer Report");
            csv.AppendLine($"Total Events,{report.TotalEvents}");
            csv.AppendLine($"Total Bookings,{report.TotalBookings}");
            csv.AppendLine($"Total Tickets Sold,{report.TotalTicketsSold}");
            csv.AppendLine($"Total Revenue,{report.TotalRevenue}");
            csv.AppendLine();
            csv.AppendLine("EventId,EventName,TotalTicketsSold,TotalRevenue");

            foreach (var item in report.TicketSales)
            {
                csv.AppendLine($"{item.EventId},{item.EventName},{item.TotalTicketsSold},{item.TotalRevenue}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            return File(bytes, "text/csv", "organizer-report.csv");
        }

        //private int? GetCurrentUserId()
        //{
        //    return HttpContext.Session.GetInt32("UserId");
        //}

        // dùng để test
        private int? GetCurrentUserId()
        {
            return 2;
        }
    }
}