using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Prn222_Nghiahnc_Razor.Pages.Customer.Rewards
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public IndexModel(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        public RewardsSummary? Summary { get; set; }

        [BindProperty]
        public string? DiscountCode { get; set; }

        [BindProperty]
        public int RedeemPoints { get; set; }

        [TempData]
        public string? Result { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var loaded = await LoadAsync();
            return loaded ? Page() : RedirectToPage("/Customer/Login", new { returnUrl = Url.Page("./Index") });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Customer/Login", new { returnUrl = Url.Page("./Index") });
            }

            var result = await _customerWorkflow.ApplyRewardAsync(userId.Value, DiscountCode, RedeemPoints);
            Result = result.Message;
            return RedirectToPage("./Index");
        }

        private async Task<bool> LoadAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return false;
            }

            Summary = await _customerWorkflow.GetRewardsSummaryAsync(userId.Value);
            return Summary != null;
        }
    }
}
