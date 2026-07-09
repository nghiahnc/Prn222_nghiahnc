using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Prn222_Nghiahnc_Mvc.Controllers
{
    public class MembershipsController : Controller
    {
        private readonly IMembershipService _membershipService;

        public MembershipsController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        // GET: /Memberships/MyMembership
        public async Task<IActionResult> MyMembership()
        {
            var userIdVal = HttpContext.Session.GetInt32("UserId");
            if (userIdVal == null)
                return RedirectToAction("Login", "Logins");

            var summary = await _membershipService.GetMembershipSummaryAsync(userIdVal.Value);
            if (summary == null)
                return NotFound("Membership details not found for this user.");

            return View(summary);
        }

        // GET: /Memberships
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 0)
                return StatusCode(StatusCodes.Status403Forbidden, "Access Denied: Admin role required.");

            var memberships = await _membershipService.GetAllAsync();
            return View(memberships);
        }
    }
}
