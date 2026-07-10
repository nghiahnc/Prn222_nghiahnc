using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Prn222_Nghiahnc_Razor.Pages.Customer.Bookings
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public IndexModel(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        public IList<Booking> Bookings { get; set; } = new List<Booking>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Customer/Login", new { returnUrl = Url.Page("./Index") });
            }

            Bookings = await _customerWorkflow.GetBookingHistoryAsync(userId.Value);
            return Page();
        }

        public string StatusLabel(int status)
        {
            return CustomerWorkflowService.BookingStatusLabel(status);
        }

        public string PaymentStatusLabel(int status)
        {
            return CustomerWorkflowService.TransactionStatusLabel(status);
        }
    }
}
