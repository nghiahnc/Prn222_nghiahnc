using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using EventEntity = Domain.Event;

namespace Prn222_Nghiahnc_Razor.Pages.Customer.Categories
{
    public class DetailsModel : PageModel
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public DetailsModel(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        public EventCategory? Category { get; set; }

        public IList<EventEntity> Events { get; set; } = new List<EventEntity>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var details = await _customerWorkflow.GetCategoryDetailsAsync(id);
            if (details == null)
            {
                return NotFound();
            }

            Category = details.Category;
            Events = details.Events;
            return Page();
        }
    }
}
