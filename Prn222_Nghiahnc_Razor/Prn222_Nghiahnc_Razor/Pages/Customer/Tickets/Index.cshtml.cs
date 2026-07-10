using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Prn222_Nghiahnc_Razor.Pages.Customer.Tickets
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public IndexModel(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        public IList<Ticket> Tickets { get; set; } = new List<Ticket>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Customer/Login", new { returnUrl = Url.Page("./Index") });
            }

            Tickets = await _customerWorkflow.GetTicketsAsync(userId.Value);
            return Page();
        }
    }
}
