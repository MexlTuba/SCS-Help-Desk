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

        public StudentController(ITicketService ticketService, ITicketRepository ticketRepository, ICategoryService categoryService, IUserService userService, IPriorityService priorityService, IStatusService statusService)
        {
            _ticketRepository = ticketRepository;
            _categoryService = categoryService;
            _priorityService = priorityService;
            _statusService = statusService;
            _userService = userService;
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

        public IActionResult MyTickets()
        {
            // Retrieve the logged-in user's UserId from claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("StudentDashboard");
            }

            // Fetch the User entity using the UserId from the claim
            var user = _userService.GetUserById(userIdClaim);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("StudentDashboard");
            }

            // Fetch tickets created by the logged-in user
            var tickets = _ticketService.GetTickets()
                .Where(ticket => ticket.CreatedBy == user.UserId) // Filter tickets by the logged-in UserId
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
                    DateClosed = ticket.DateClosed,
                })
                .ToList();

            // Create the wrapper view model with both tickets and filter options
            var model = new MyTicketsViewModel
            {
                Tickets = tickets,
                Categories = _categoryService.GetAllCategories(),
                Statuses = _statusService.GetAllStatuses(),
                Priorities = _priorityService.GetAllPriorities()
            };

            return View(model); // Pass the model to the view
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
            
            return View();
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
