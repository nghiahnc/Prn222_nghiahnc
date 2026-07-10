using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Prn222_Nghiahnc_Razor.Pages.Customer.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public IndexModel(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        public IList<EventCategory> Categories { get; set; } = new List<EventCategory>();

        public async Task OnGetAsync()
        {
            Categories = await _customerWorkflow.GetCategoriesAsync();
        }
    }
}
