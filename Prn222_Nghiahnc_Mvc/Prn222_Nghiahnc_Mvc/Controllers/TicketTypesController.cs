using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Services;
using Prn222_Nghiahnc_Mvc.Models;

namespace Prn222_Nghiahnc_Mvc.Controllers
{
    public class TicketTypesController : Controller
    {
        private readonly ITicketTypeService _ticketTypeService;
        private readonly IEventService _eventService;

        public TicketTypesController(ITicketTypeService ticketTypeService, IEventService eventService)
        {
            _ticketTypeService = ticketTypeService;
            _eventService = eventService;
        }

        private int CurrentUserId => HttpContext.Session.GetInt32("UserId") ?? 0;
        private int CurrentUserRole => HttpContext.Session.GetInt32("Role") ?? -1;

        private async Task<bool> CanManageEventAsync(int eventId)
        {
            var role = CurrentUserRole;
            if (role == 0) return true;
            if (role == 2)
            {
                var ev = await _eventService.GetDetailsAsync(eventId);
                if (ev == null) return false;
                
                // Block if Cancelled or Started
                if (ev.Status == 2) return false;
                if (ev.TimeStart <= DateTime.Now) return false;

                return ev.CreatedBy == CurrentUserId;
            }
            return false;
        }

        private async Task<bool> CanManageEventForTicketAsync(int ticketTypeId)
        {
            var role = CurrentUserRole;
            if (role == 0) return true;
            var tt = await _ticketTypeService.GetDetailsAsync(ticketTypeId);
            if (tt == null) return false;
            return await CanManageEventAsync(tt.EventId);
        }

        private async Task<System.Collections.Generic.List<Event>> BuildEventDropdownAsync(int currentUserId, int currentUserRole)
        {
            var allEvents = await _eventService.GetAllAsync();
            var validEvents = new System.Collections.Generic.List<Event>();
            foreach (var ev in allEvents)
            {
                if (ev.Status != 2 && ev.TimeStart > DateTime.Now)
                {
                    if (currentUserRole == 0 || (currentUserRole == 2 && ev.CreatedBy == currentUserId))
                    {
                        validEvents.Add(ev);
                    }
                }
            }
            return validEvents;
        }

        // GET: /TicketTypes
        public async Task<IActionResult> Index(int? eventId)
        {
            if (eventId.HasValue)
            {
                if (!await CanManageEventAsync(eventId.Value))
                    return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");
                
                var ev = await _eventService.GetDetailsAsync(eventId.Value);
                ViewBag.EventName = ev?.Name;
                var types = await _ticketTypeService.GetByEventIdAsync(eventId.Value);
                return View(types);
            }
            else
            {
                if (CurrentUserRole != 0)
                    return StatusCode(StatusCodes.Status403Forbidden, "Access Denied: Admin role required.");
                
                var types = await _ticketTypeService.GetAllAsync();
                return View(types);
            }
        }

        // GET: /TicketTypes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!await CanManageEventForTicketAsync(id))
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");

            var details = await _ticketTypeService.GetDetailsAsync(id);
            if (details == null)
                return NotFound();

            return View(details);
        }

        // GET: /TicketTypes/Create
        public async Task<IActionResult> Create(int? eventId)
        {
            var role = CurrentUserRole;
            if (role != 0 && role != 2)
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");

            if (eventId.HasValue)
            {
                if (!await CanManageEventAsync(eventId.Value))
                    return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");
            }

            var vm = new TicketTypeFormViewModel
            {
                EventId = eventId ?? 0,
                Events = await BuildEventDropdownAsync(CurrentUserId, CurrentUserRole)
            };
            return View(vm);
        }

        // POST: /TicketTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketTypeFormViewModel vm)
        {
            var role = CurrentUserRole;
            if (role != 0 && role != 2)
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");

            if (!ModelState.IsValid)
            {
                vm.Events = await BuildEventDropdownAsync(CurrentUserId, CurrentUserRole);
                return View(vm);
            }

            var request = new TicketTypeRequest
            {
                EventId = vm.EventId,
                Name = vm.Name,
                Price = vm.Price,
                Quantity = vm.Quantity,
                CurrentUserId = CurrentUserId,
                CurrentUserRole = CurrentUserRole
            };

            var result = await _ticketTypeService.CreateAsync(request);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index), new { eventId = vm.EventId });
            }

            ModelState.AddModelError("", result.Error ?? "Error occurred during ticket type creation.");
            vm.Events = await BuildEventDropdownAsync(CurrentUserId, CurrentUserRole);
            return View(vm);
        }

        // GET: /TicketTypes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!await CanManageEventForTicketAsync(id))
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");

            var details = await _ticketTypeService.GetDetailsAsync(id);
            if (details == null)
                return NotFound();

            var vm = new TicketTypeFormViewModel
            {
                Id = details.Id,
                EventId = details.EventId,
                Name = details.Name,
                Price = details.Price,
                Quantity = details.Quantity,
                Events = await BuildEventDropdownAsync(CurrentUserId, CurrentUserRole)
            };
            return View(vm);
        }

        // POST: /TicketTypes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TicketTypeFormViewModel vm)
        {
            var role = CurrentUserRole;
            if (role != 0 && role != 2)
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");

            if (!ModelState.IsValid)
            {
                vm.Events = await BuildEventDropdownAsync(CurrentUserId, CurrentUserRole);
                return View(vm);
            }

            var request = new TicketTypeRequest
            {
                Id = vm.Id,
                EventId = vm.EventId,
                Name = vm.Name,
                Price = vm.Price,
                Quantity = vm.Quantity,
                CurrentUserId = CurrentUserId,
                CurrentUserRole = CurrentUserRole
            };

            var result = await _ticketTypeService.UpdateAsync(request);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index), new { eventId = vm.EventId });
            }

            ModelState.AddModelError("", result.Error ?? "Error occurred during ticket type update.");
            vm.Events = await BuildEventDropdownAsync(CurrentUserId, CurrentUserRole);
            return View(vm);
        }

        // GET: /TicketTypes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (!await CanManageEventForTicketAsync(id))
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");

            var details = await _ticketTypeService.GetDetailsAsync(id);
            if (details == null)
                return NotFound();

            return View(details);
        }

        // POST: /TicketTypes/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await CanManageEventForTicketAsync(id))
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");

            var details = await _ticketTypeService.GetDetailsAsync(id);
            if (details == null)
                return NotFound();

            var eventId = details.EventId;
            var result = await _ticketTypeService.DeleteAsync(id, CurrentUserId, CurrentUserRole);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index), new { eventId = eventId });
            }

            TempData["Error"] = result.Error ?? "Error occurred during ticket type deletion.";
            return RedirectToAction(nameof(Index), new { eventId = eventId });
        }
    }
}
