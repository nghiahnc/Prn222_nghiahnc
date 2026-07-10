using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Prn222_Nghiahnc_Razor.Pages.Customer.Tickets
{
    public class ScanModel : PageModel
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public ScanModel(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        [BindProperty]
        public string? Qr { get; set; }

        public string? Message { get; set; }

        public Ticket? Ticket { get; set; }

        public void OnGet()
        {
        }

        public async Task OnPostAsync()
        {
            var result = await _customerWorkflow.ScanTicketAsync(Qr);
            Message = result.Message;
            Ticket = result.Ticket;
        }
    }
}
