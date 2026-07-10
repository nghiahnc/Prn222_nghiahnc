using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Prn222_Nghiahnc_Razor.Pages.Customer.Bookings
{
    public class DetailsModel : PageModel
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public DetailsModel(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        public BookingDetailsResult? Details { get; set; }

        [TempData]
        public string? Message { get; set; }

        [TempData]
        public string? Error { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            return await LoadAsync(id);
        }

        public async Task<IActionResult> OnPostConfirmPaymentAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Customer/Login", new { returnUrl = Url.Page("./Details", new { id }) });
            }

            var result = await _customerWorkflow.ConfirmTransactionAsync(userId.Value, id);
            if (!result.Success)
            {
                Error = result.Error;
                return RedirectToPage("./Details", new { id });
            }

            Message = result.Message;
            return RedirectToPage("./Details", new { id = result.BookingId });
        }

        public string PaymentStatusLabel(int status)
        {
            return CustomerWorkflowService.TransactionStatusLabel(status);
        }

        public string StatusLabel(int status)
        {
            return CustomerWorkflowService.BookingStatusLabel(status);
        }

        private async Task<IActionResult> LoadAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Customer/Login", new { returnUrl = Url.Page("./Details", new { id }) });
            }

            Details = await _customerWorkflow.GetBookingDetailsAsync(userId.Value, id);
            return Details == null ? NotFound() : Page();
        }
    }
}
