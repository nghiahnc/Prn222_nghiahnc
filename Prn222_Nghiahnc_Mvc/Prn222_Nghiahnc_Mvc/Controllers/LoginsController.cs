using Microsoft.AspNetCore.Mvc;
using Prn222_Nghiahnc_Mvc.Models;
using Services;

namespace Prn222_Nghiahnc_Mvc.Controllers
{
    public class LoginsController : Controller
    {
        private readonly ICustomerWorkflowService _customerWorkflow;

        public LoginsController(ICustomerWorkflowService customerWorkflow)
        {
            _customerWorkflow = customerWorkflow;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _customerWorkflow.LoginAsync(model.UserName, model.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Sai tai khoan hoac mat khau");
                return View(model);
            }

            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetInt32("Role", user.Role);

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Bookings");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}
