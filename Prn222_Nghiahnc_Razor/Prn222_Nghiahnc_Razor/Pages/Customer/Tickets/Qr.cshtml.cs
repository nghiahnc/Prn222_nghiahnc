using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Prn222_Nghiahnc_Razor.Pages.Customer.Tickets
{
    public class QrModel : PageModel
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public QrModel(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        public Ticket? Ticket { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Customer/Login", new { returnUrl = Url.Page("./Qr", new { id }) });
            }

            Ticket = await _customerWorkflow.GetOwnedTicketAsync(userId.Value, id, ensureQr: true);
            return Ticket == null ? NotFound() : Page();
        }
    }
}
