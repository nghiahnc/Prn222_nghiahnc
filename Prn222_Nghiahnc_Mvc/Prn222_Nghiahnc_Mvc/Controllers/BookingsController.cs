using Domain;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Prn222_Nghiahnc_Mvc.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,UserId,Status,CreatedAt,TransactionId")] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            await _bookingService.CreateBookingAsync(booking);

            return RedirectToAction(nameof(Index));
        }
    }
}
