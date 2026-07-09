using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Services;
using Prn222_Nghiahnc_Mvc.Models;

namespace Prn222_Nghiahnc_Mvc.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        private int CurrentUserId => HttpContext.Session.GetInt32("UserId") ?? 0;
        private int CurrentUserRole => HttpContext.Session.GetInt32("Role") ?? -1;

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        private bool IsAdminOrOrganizer()
        {
            return CurrentUserRole == 0 || CurrentUserRole == 2;
        }

        // GET: /Events
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");

            var events = await _eventService.GetAllAsync();
            return View(events);
        }

        // GET: /Events/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");

            var ev = await _eventService.GetDetailsAsync(id);
            if (ev == null)
                return NotFound();

            return View(ev);
        }

        // GET: /Events/Create
        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");
            if (!IsAdminOrOrganizer())
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied: Admin or Organizer role required.");

            var vm = new EventFormViewModel
            {
                Categories = await _eventService.GetCategoriesAsync()
            };
            return View(vm);
        }

        // POST: /Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventFormViewModel vm)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");
            if (!IsAdminOrOrganizer())
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied: Admin or Organizer role required.");

            var request = new CreateEventRequest
            {
                Name = vm.Name,
                EventCategoryId = vm.EventCategoryId,
                Detail = vm.Detail,
                Location = vm.Location,
                TimeStart = vm.TimeStart,
                TimeEnd = vm.TimeEnd,
                CreatedBy = CurrentUserId
            };

            var result = await _eventService.CreateAsync(request);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", result.Error ?? "Error occurred during event creation.");
            vm.Categories = await _eventService.GetCategoriesAsync();
            return View(vm);
        }

        // GET: /Events/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");

            var ev = await _eventService.GetDetailsAsync(id);
            if (ev == null)
                return NotFound();

            if (CurrentUserRole != 0 && (CurrentUserRole != 2 || ev.CreatedBy != CurrentUserId))
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied: You do not have permission to manage this event.");

            var vm = new EventFormViewModel
            {
                Id = ev.Id,
                Name = ev.Name,
                EventCategoryId = ev.EventCategoryId,
                Detail = ev.Detail,
                Location = ev.Location,
                TimeStart = ev.TimeStart,
                TimeEnd = ev.TimeEnd,
                Categories = await _eventService.GetCategoriesAsync()
            };

            return View(vm);
        }

        // POST: /Events/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventFormViewModel vm)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");

            var request = new UpdateEventRequest
            {
                Id = vm.Id,
                Name = vm.Name,
                EventCategoryId = vm.EventCategoryId,
                Detail = vm.Detail,
                Location = vm.Location,
                TimeStart = vm.TimeStart,
                TimeEnd = vm.TimeEnd,
                CurrentUserId = CurrentUserId,
                CurrentUserRole = CurrentUserRole
            };

            var result = await _eventService.UpdateAsync(request);
            if (result.Success)
            {
                return RedirectToAction(nameof(Details), new { id = vm.Id });
            }

            ModelState.AddModelError("", result.Error ?? "Error occurred during event update.");
            vm.Categories = await _eventService.GetCategoriesAsync();
            return View(vm);
        }

        // GET: /Events/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");

            var ev = await _eventService.GetDetailsAsync(id);
            if (ev == null)
                return NotFound();

            if (CurrentUserRole != 0 && (CurrentUserRole != 2 || ev.CreatedBy != CurrentUserId))
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied: You do not have permission to manage this event.");

            return View(ev);
        }

        // POST: /Events/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");

            var result = await _eventService.DeleteAsync(id, CurrentUserId, CurrentUserRole);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = result.Error ?? "Error occurred during event deletion.";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // POST: /Events/Publish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");

            var result = await _eventService.PublishAsync(id, CurrentUserId, CurrentUserRole);
            if (result.Success)
            {
                TempData["Success"] = "Event published successfully.";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Error occurred during event publishing.";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        // GET: /Events/Cancel/5
        public async Task<IActionResult> Cancel(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");

            var ev = await _eventService.GetDetailsAsync(id);
            if (ev == null)
                return NotFound();

            if (CurrentUserRole != 0 && (CurrentUserRole != 2 || ev.CreatedBy != CurrentUserId))
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied: You do not have permission to manage this event.");

            return View(ev);
        }

        // POST: /Events/Cancel/5
        [HttpPost]
        [ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");

            var result = await _eventService.CancelAsync(id, CurrentUserId, CurrentUserRole);
            if (result.Success)
            {
                TempData["Success"] = "Event cancelled successfully.";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            TempData["Error"] = result.Error ?? "Error occurred during event cancellation.";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // GET: /Events/Postpone/5
        public async Task<IActionResult> Postpone(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");

            var ev = await _eventService.GetDetailsAsync(id);
            if (ev == null)
                return NotFound();

            if (CurrentUserRole != 0 && (CurrentUserRole != 2 || ev.CreatedBy != CurrentUserId))
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied: You do not have permission to manage this event.");

            var vm = new PostponeEventViewModel
            {
                EventId = ev.Id,
                CurrentTimeStart = ev.TimeStart,
                CurrentTimeEnd = ev.TimeEnd,
                NewTimeStart = ev.TimeStart,
                NewTimeEnd = ev.TimeEnd
            };
            return View(vm);
        }

        // POST: /Events/Postpone/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Postpone(PostponeEventViewModel vm)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Logins");

            var request = new PostponeEventRequest
            {
                EventId = vm.EventId,
                NewTimeStart = vm.NewTimeStart,
                NewTimeEnd = vm.NewTimeEnd,
                Reason = vm.Reason,
                CurrentUserId = CurrentUserId,
                CurrentUserRole = CurrentUserRole
            };

            var result = await _eventService.PostponeAsync(request);
            if (result.Success)
            {
                TempData["Success"] = "Event postponed successfully.";
                return RedirectToAction(nameof(Details), new { id = vm.EventId });
            }

            ModelState.AddModelError("", result.Error ?? "Error occurred during event postponement.");
            return View(vm);
        }
    }
}
