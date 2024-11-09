using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ASI.Basecode.WebApp.Controllers
{
    public class StudentController : Controller
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICategoryService _categoryService; // To populate categories dropdown
        private readonly IUserService _userService;         // To get current user information

        public StudentController(ITicketRepository ticketRepository, ICategoryService categoryService, IUserService userService)
        {
            _ticketRepository = ticketRepository;
            _categoryService = categoryService;
            _userService = userService;
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
            var name = User.Identity.Name;
            var user = _userService.GetUserByName(name);

            if (!ModelState.IsValid)
            {
                // Repopulate categories for the dropdown on model validation failure
                model.Categories = _categoryService.GetAllCategories();
                return View(model);
            }

            try
            {
                // Create a new Ticket from the ViewModel
                var ticket = new Ticket
                {
                    Title = model.Title,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Attatchment = model.Attatchment,
                    PriorityId = 4,      // Default to "General" priority
                    StatusId = 1,        // Default to "Open" status
                    CreatedBy = user.UserId, // Get the ID of the logged-in user

                    DateCreated = DateTime.Now
                };

                // Save the ticket to the database
                _ticketRepository.AddTicket(ticket);

                // Set a success message in TempData
                TempData["SuccessMessage"] = "Ticket created successfully!";

                // Redirect to the Student's ticket list or confirmation page
                return RedirectToAction("MyTickets", "Student");
            }
            catch (Exception ex)
            {
                // Capture any exception, log it, and show an error message
                TempData["ErrorMessage"] = "An error occurred while creating the ticket. Please try again.";
                return View(model);
            }
        }

        public IActionResult MyTickets()
        {
            // Code to display student's tickets (this can be implemented as needed)
            return View();
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
    }
}
