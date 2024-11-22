using ASI.Basecode.Data.Models;
using ASI.Basecode.Services;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly IPriorityService _priorityService;
        private readonly IStatusService _statusService;
        private readonly ITicketService _ticketService;
        public AdminController(IUserService userService, ICategoryService categoryService, IPriorityService priorityService, IStatusService statusService, ITicketService ticketService)
        {
            _userService = userService;
            _categoryService = categoryService;
            _priorityService = priorityService;
            _statusService = statusService;
            _ticketService = ticketService;
        }
        public IActionResult AdminDashboard()
        {
            return View();
        }
        // GET: UsersController
        public ActionResult UserList()
        {
            var users = _userService.GetAllUsers().Where(u => u.Role != "Super Admin" && u.Role != "Admin").ToList();
            return View(users);
        }

        public ActionResult Tickets()
        {
            var model = new TicketViewModel
            {
                Tickets = _ticketService.GetAllTickets(),
                Categories = _categoryService.GetAllCategories(),  // Load categories for dropdown
                Priorities = _priorityService.GetAllPriorities(),  // Load priorities for dropdown
                Statuses = _statusService.GetAllStatuses()    //Load statuses for dropdown
            };
            return View(model);
        }

        public ActionResult TicketDetails(int id)
        {
            var ticket = _ticketService.GetTicketById(id);

            if (ticket == null)
            {
                return NotFound();
            }
            var supportAgents = _userService.GetAllUsers()
                                            .Where(u => u.Role == "SupportAgent") // Filter by role
                                            .Select(u => new { u.UserId, u.Name })
                                            .ToList();

            ViewBag.SupportAgents = new SelectList(supportAgents, "UserId", "Name");
            ViewBag.Ticket = ticket;
            return View();
        }

        // GET: UsersController/UserAdd
        public ActionResult UserAdd()
        {
            return View();
        }

        // POST: SuperAdminController/RegisterUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserAdd(UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _userService.AddUser(model);
                    return RedirectToAction(nameof(UserList));
                }
                return View(model);
            }
            catch
            {
                return View(model);
            }
        }

        // GET: UsersController/Edit/UserId
        public ActionResult Edit(string id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: SuperAdminController/Edit/userId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _userService.UpdateUser(user);
                    return RedirectToAction(nameof(UserList));
                }
                return View(user);
            }
            catch
            {
                return View(user);
            }
        }

        // POST: SuperAdminController/ResetPassword/userId
        [HttpPost]
        public ActionResult ResetPassword(string id)
        {
            try
            {
                _userService.ResetPassword(id, "Temp_123");
                return RedirectToAction(nameof(UserList));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(string id)
        {
            try
            {
                _userService.DeleteUser(id);
                return RedirectToAction(nameof(UserList));
            }
            catch
            {
                return View();
            }
        }

        // POST: UsersController/Delete/userId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(UserList));
            }
            catch
            {
                return View();
            }
        }
    }
}
