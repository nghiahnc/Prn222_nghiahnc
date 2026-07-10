using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Prn222_Nghiahnc_Razor.Pages.Customer
{
    public class LoginModel : PageModel
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public LoginModel(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        [BindProperty]
        public string UserName { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public string? Error { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _customerWorkflow.LoginAsync(UserName, Password);
            if (user == null)
            {
                Error = "Sai tai khoan hoac mat khau";
                ModelState.AddModelError(string.Empty, "Sai tai khoan hoac mat khau");
                return Page();
            }

            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetInt32("Role", user.Role);

            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return LocalRedirect(ReturnUrl);
            }

            return RedirectToPage("/Customer/Events/Index");
        }
    }
}
