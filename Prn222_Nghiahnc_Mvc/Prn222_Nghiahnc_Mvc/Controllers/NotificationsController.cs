using Microsoft.AspNetCore.Mvc;
using Prn222_Nghiahnc_Mvc.ViewModels;
using Services;

namespace Prn222_Nghiahnc_Mvc.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IActionResult Send()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(MassNotificationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _notificationService.SendMassNotificationAsync(
                model.Subject,
                model.Message);

            ViewBag.Message = "Đã gửi thông báo thành công.";
            return View();
        }
    }
}
