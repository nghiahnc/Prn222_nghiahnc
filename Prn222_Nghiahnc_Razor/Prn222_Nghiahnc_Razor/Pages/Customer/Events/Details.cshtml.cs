using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using EventEntity = Domain.Event;

namespace Prn222_Nghiahnc_Razor.Pages.Customer.Events
{
    public class DetailsModel : PageModel
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public DetailsModel(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        public EventEntity? EventItem { get; set; }

        public IList<TicketType> TicketTypes { get; set; } = new List<TicketType>();

        [TempData]
        public string? Message { get; set; }

        [TempData]
        public string? Error { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var details = await _customerWorkflow.GetEventDetailsAsync(id);
            if (details == null)
            {
                return NotFound();
            }

            EventItem = details.EventItem;
            TicketTypes = details.TicketTypes;
            return Page();
        }

        public async Task<IActionResult> OnPostBookTicketAsync(int id, int ticketTypeId, int quantity = 1, string? discountCode = null, int redeemPoints = 0)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Customer/Login", new { returnUrl = Url.Page("./Details", new { id }) });
            }

            var result = await _customerWorkflow.BookTicketAsync(userId.Value, ticketTypeId, quantity, discountCode, redeemPoints);
            if (!result.Success)
            {
                Error = result.Error;
                return RedirectToPage("./Details", new { id });
            }

            Message = "Booking created. Please confirm the transaction to issue your QR ticket.";
            return RedirectToPage("/Customer/Bookings/Details", new { id = result.BookingId });
        }

        public string StatusLabel(int status)
        {
            return CustomerWorkflowService.EventStatusLabel(status);
        }
    }
}
