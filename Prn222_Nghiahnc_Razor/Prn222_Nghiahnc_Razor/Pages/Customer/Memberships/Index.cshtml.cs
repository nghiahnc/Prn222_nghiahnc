using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Prn222_Nghiahnc_Razor.Pages.Customer.Memberships
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public IndexModel(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        public IList<Membership> Memberships { get; set; } = new List<Membership>();

        public async Task OnGetAsync()
        {
            Memberships = await _customerWorkflow.GetMembershipsAsync();
        }

        public int Threshold(Membership membership)
        {
            return CustomerWorkflowService.GetTierThreshold(membership);
        }

        public int Discount(Membership membership)
        {
            return CustomerWorkflowService.GetTierDiscountPercent(membership);
        }
    }
}
