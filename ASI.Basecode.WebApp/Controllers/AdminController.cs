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
        private readonly IUserPreferencesService _userPreferencesService;
        private readonly ICategoryService _categoryService;
        private readonly IPriorityService _priorityService; 
        private readonly IStatusService _statusService;
        private readonly ITicketService _ticketService;
        public AdminController(IUserService userService, IUserPreferencesService userPreferencesService, ICategoryService categoryService, IPriorityService priorityService, IStatusService statusService, ITicketService ticketService)
        {
            _userService = userService;
            _userPreferencesService = userPreferencesService;
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
            var users = _userService.GetAllUsers(); // Fetch users from the service
            return View(users);
        }

        public ActionResult Tickets(int? categoryId = null, int? statusId = null, int? priorityId = null)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect if session expired
            }

            // Get user preferences
            var preferences = _userPreferencesService.GetPreferencesByUserId(userId);
            var defaultCategoryId = preferences?.DefaultCategoryId ?? 1;
            var defaultPriorityId = preferences?.DefaultPriorityId ?? 4;
            var defaultStatusId = preferences?.DefaultStatusId ?? 1;

            // Use user preferences as defaults if no filter is selected or "All" (0) is selected
            var appliedCategoryId = categoryId == 0 || categoryId == null ? defaultCategoryId : categoryId.Value;
            var appliedStatusId = statusId == 0 || statusId == null ? defaultStatusId : statusId.Value;
            var appliedPriorityId = priorityId == 0 || priorityId == null ? defaultPriorityId : priorityId.Value;

            // Filter tickets
            var tickets = _ticketService.GetAllTickets()
                                        .Where(t => (categoryId == 0 || t.CategoryId == appliedCategoryId) &&
                                                    (statusId == 0 || t.StatusId == appliedStatusId) &&
                                                    (priorityId == 0 || t.PriorityId == appliedPriorityId))
                                        .ToList();

            // Prepare the view model
            var model = new TicketViewModel
            {
                Tickets = tickets,
                Categories = _categoryService.GetAllCategories(),
                Priorities = _priorityService.GetAllPriorities(),
                Statuses = _statusService.GetAllStatuses(),
                CategoryId = categoryId ?? defaultCategoryId, // Preserve selected filter or fallback to user default
                PriorityId = priorityId ?? defaultPriorityId,
                StatusId = statusId ?? defaultStatusId
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
        public IActionResult Settings()
        {
            // Retrieve UserId from session
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if session expired
            }

            // Fetch user preferences using the service
            var preferences = _userPreferencesService.GetPreferencesByUserId(userId);

            if (preferences == null)
            {
                // Handle case where preferences do not exist (e.g., initialize defaults)
                preferences = new UserPreferences
                {
                    UserId = userId,
                    DefaultCategoryId = 1,
                    DefaultStatusId = 1,
                    DefaultPriorityId = 4
                };
                _userPreferencesService.AddPreferences(preferences);
            }

            // Map preferences to ViewModel
            var model = new UserPreferencesViewModel
            {
                UserId = preferences.UserId,
                DefaultCategoryId = preferences.DefaultCategoryId,
                DefaultStatusId = preferences.DefaultStatusId,
                DefaultPriorityId = preferences.DefaultPriorityId
            };

            return View(model);
        }

    }
}
