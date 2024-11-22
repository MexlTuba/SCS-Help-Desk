using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class StudentController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ITicketRepository _ticketRepository;
        private readonly ICategoryService _categoryService; // To populate categories dropdown
        private readonly IPriorityService _priorityService;
        private readonly IStatusService _statusService;
        private readonly IUserService _userService;         // To get current user information
        private readonly IUserPreferencesService _userPreferencesService;

        public StudentController(ITicketService ticketService, ITicketRepository ticketRepository, ICategoryService categoryService, IUserPreferencesService userPreferencesService, IUserService userService, IPriorityService priorityService, IStatusService statusService)
        {
            _ticketRepository = ticketRepository;
            _categoryService = categoryService;
            _priorityService = priorityService;
            _statusService = statusService;
            _userService = userService;
            _userPreferencesService = userPreferencesService;
            _ticketService = ticketService;
        }

        [HttpGet]
        public IActionResult CreateTicket()
        {
            var model = new TicketViewModel
            {
                Categories = _categoryService.GetAllCategories()  // Load categories for dropdown
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTicket(TicketViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.Name; // Retrieve the logged-in username
                    _ticketService.AddTicket(model, userName);

                    // Set a success message in TempData
                    TempData["SuccessMessage"] = "Ticket created successfully!";
                    return RedirectToAction("MyTickets", "Student");
                }

                // Repopulate categories for the dropdown if model validation fails
                model.Categories = _categoryService.GetAllCategories();
                model.Statuses = _statusService.GetAllStatuses();
                model.Priorities = _priorityService.GetAllPriorities();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the ticket. Please try again.";
                return View(model);
            }
        }

        public IActionResult MyTickets(int? categoryId = null, int? statusId = null, int? priorityId = null)
        {
            // Retrieve the logged-in user's UserId
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("StudentDashboard");
            }

            // Fetch user preferences for defaults
            var preferences = _userPreferencesService.GetPreferencesByUserId(userIdClaim);
            var defaultCategoryId = preferences?.DefaultCategoryId ?? 1; 
            var defaultPriorityId = preferences?.DefaultPriorityId ?? 4;
            var defaultStatusId = preferences?.DefaultStatusId ?? 1;

            // Resolve filter values
            var appliedCategoryId = categoryId == null ? defaultCategoryId : categoryId.Value;
            var appliedStatusId = statusId == null ? defaultStatusId : statusId.Value;
            var appliedPriorityId = priorityId == null ? defaultPriorityId : priorityId.Value;

            // Fetch and filter tickets
            var tickets = _ticketService.GetTickets()
                .Where(ticket => ticket.CreatedBy == userIdClaim) // Only user's tickets
                .Where(ticket => appliedCategoryId == 0 || ticket.CategoryId == appliedCategoryId) // Include all if 0
                .Where(ticket => appliedStatusId == 0 || ticket.StatusId == appliedStatusId)
                .Where(ticket => appliedPriorityId == 0 || ticket.PriorityId == appliedPriorityId)
                .Select(ticket => new TicketViewModel
                {
                    TicketId = ticket.TicketId,
                    Title = ticket.Title,
                    AssignedTo = ticket.AssignedTo,
                    Description = ticket.Description,
                    CategoryId = ticket.CategoryId,
                    PriorityId = ticket.PriorityId,
                    StatusId = ticket.StatusId,
                    DateCreated = ticket.DateCreated,
                    DateClosed = ticket.DateClosed
                })
                .ToList();

            // Prepare the view model
            var model = new MyTicketsViewModel
            {
                Tickets = tickets,
                Categories = _categoryService.GetAllCategories(),
                Statuses = _statusService.GetAllStatuses(),
                Priorities = _priorityService.GetAllPriorities(),
                CategoryId = categoryId ?? defaultCategoryId, // Persist selected filter or fallback to defaults
                PriorityId = priorityId ?? defaultPriorityId,
                StatusId = statusId ?? defaultStatusId
            };

            return View(model);
        }





        public IActionResult StudentDashboard()
        {
            return View();
        }

        public IActionResult KnowledgeBase()
        {
            return View();
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

        // GET: StudentController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                _ticketService.DeleteTicket(id);
                return RedirectToAction(nameof(MyTickets));
            }
            catch
            {
                return View();
            }
        }

        // POST: StudentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(MyTickets));
            }
            catch
            {
                return View();
            }
        }


    }
}
