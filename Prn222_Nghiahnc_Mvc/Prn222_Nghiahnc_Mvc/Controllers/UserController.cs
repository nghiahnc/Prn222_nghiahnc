using Domain;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        // GET: /User
        public IActionResult Index()
        {
            var users = _service.GetAllUsers();
            return View(users);
        }

        // GET: /User/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /User/Create
        [HttpPost]
        public IActionResult Create(User user)
        {
            _service.CreateUser(user);
            return RedirectToAction(nameof(Index));
        }

        // GET: /User/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _service.GetUserById(id);
            return View(user);
        }

        // POST: /User/Edit
        [HttpPost]
        public IActionResult Edit(User user)
        {
            _service.UpdateUser(user);
            return RedirectToAction(nameof(Index));
        }

        // GET: /User/Delete/5
        public IActionResult Delete(int id)
        {
            _service.DeleteUser(id);
            return RedirectToAction(nameof(Index));
        }
    }
}