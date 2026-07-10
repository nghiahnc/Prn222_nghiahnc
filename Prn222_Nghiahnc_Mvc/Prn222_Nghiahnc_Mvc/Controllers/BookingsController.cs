using Microsoft.AspNetCore.Mvc;
using Prn222_Nghiahnc_Mvc.Models;
using Services;

namespace Prn222_Nghiahnc_Mvc.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ICustomerWorkflowService _customerWorkflow;
        private readonly IConfiguration _configuration;

        public BookingsController(ICustomerWorkflowService customerWorkflow, IConfiguration configuration)
        {
            _customerWorkflow = customerWorkflow;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Logins", new { returnUrl = Url.Action("Index", "Bookings") });
            }

            return View(new BookingHistoryViewModel
            {
                Bookings = await _customerWorkflow.GetBookingHistoryAsync(userId.Value)
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Logins", new { returnUrl = Url.Action("Details", "Bookings", new { id }) });
            }

            var details = await _customerWorkflow.GetBookingDetailsAsync(userId.Value, id);
            if (details == null)
            {
                return NotFound();
            }

            return View(new BookingDetailsViewModel
            {
                Details = details,
                RazorBaseUrl = (_configuration["RazorApp:BaseUrl"] ?? string.Empty).TrimEnd('/')
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Logins", new { returnUrl = Url.Action("Details", "Bookings", new { id }) });
            }

            var result = await _customerWorkflow.ConfirmTransactionAsync(userId.Value, id);
            if (!result.Success)
            {
                TempData["Error"] = result.Error;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["Message"] = result.Message;
            return RedirectToAction(nameof(Details), new { id = result.BookingId });
        }
    }
}
