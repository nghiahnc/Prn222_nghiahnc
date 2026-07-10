using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;

namespace Prn222_Nghiahnc_Razor.Pages.Customer.Events
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public IndexModel(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? DateFilter { get; set; } = "upcoming";

        [BindProperty(SupportsGet = true)]
        public string? Location { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? TicketTypeId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; } = 1;

        public IList<EventListItem> Events { get; set; } = new List<EventListItem>();

        public IList<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();

        public IList<SelectListItem> TicketTypeOptions { get; set; } = new List<SelectListItem>();

        public IList<SelectListItem> LocationOptions { get; set; } = new List<SelectListItem>();

        public async Task OnGetAsync()
        {
            var result = await _customerWorkflow.SearchEventsAsync(
                new EventSearchRequest(Search, CategoryId, DateFilter, Location, TicketTypeId, PageNumber));

            Events = result.Items;
            PageNumber = result.PageNumber;
            TotalPages = result.TotalPages;
            CategoryOptions = result.Categories
                .Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == CategoryId))
                .ToList();
            TicketTypeOptions = result.TicketTypes
                .Select(t => new SelectListItem(t.Name, t.Id.ToString(), t.Id == TicketTypeId))
                .ToList();
            LocationOptions = result.Locations
                .Select(x => new SelectListItem(x, x, x == Location))
                .ToList();
        }

        public string StatusLabel(int status)
        {
            return CustomerWorkflowService.EventStatusLabel(status);
        }
    }
}
