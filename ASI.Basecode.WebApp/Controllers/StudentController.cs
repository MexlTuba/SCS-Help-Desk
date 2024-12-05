using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
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

                    // File upload handling
                    string filePath = null;
                    if (model.Attachment != null)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Attachments");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Attachment.FileName);
                        filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            model.Attachment.CopyTo(stream);
                        }

                        // Convert file path to a relative path for database storage
                        filePath = Path.Combine("Attachments", uniqueFileName);

                        // Normalize to use forward slashes
                        filePath = filePath.Replace("\\", "/");
                    }

                    // Map ViewModel to ServiceModel and include the normalized file path
                    model.AttachmentPath = filePath;

                    // Pass the updated model and userName to the service
                    _ticketService.AddTicket(model, userName);

                    TempData["SuccessMessage"] = "Ticket created successfully!";
                    return RedirectToAction("MyTickets", "Student");
                }

                model.Categories = _categoryService.GetAllCategories();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the ticket. Please try again.";
                return View(model);
            }
        }




        public IActionResult MyTickets(int? categoryId = null, int? statusId = null, int? priorityId = null, int page = 1, int pageSize = 5)
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
            var ticketsQuery = _ticketService.GetTickets()
                .Where(ticket => ticket.CreatedBy == userIdClaim) // Only user's tickets
                .Where(ticket => appliedCategoryId == 0 || ticket.CategoryId == appliedCategoryId) // Include all if 0
                .Where(ticket => appliedStatusId == 0 || ticket.StatusId == appliedStatusId)
                .Where(ticket => appliedPriorityId == 0 || ticket.PriorityId == appliedPriorityId)
                .Where(ticket => ticket.StatusId != 5);

            // Pagination
            var totalTickets = ticketsQuery.Count();
            var totalPages = (int)Math.Ceiling(totalTickets / (double)pageSize);

            var tickets = ticketsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ticket =>
                {
                    string assignedToName = null;
                    if (ticket.AssignedTo.HasValue)
                    {
                        var assignedUser = _userService.GetUserById(ticket.AssignedTo.Value.ToString());
                        assignedToName = assignedUser?.Name;
                    }

                    return new TicketViewModel
                    {
                        TicketId = ticket.TicketId,
                        Title = ticket.Title,
                        AssignedTo = ticket.AssignedTo,
                        AssignedToName = assignedToName, // Include name of assigned user
                        Description = ticket.Description,
                        CategoryId = ticket.CategoryId,
                        PriorityId = ticket.PriorityId,
                        StatusId = ticket.StatusId,
                        DateCreated = ticket.DateCreated,
                        DateClosed = ticket.DateClosed
                    };
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
                StatusId = statusId ?? defaultStatusId,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult DetailsTicket(int id)
        {
            var ticket = _ticketService.GetTicketById(id);
            if (ticket == null)
            {
                TempData["ErrorMessage"] = "Ticket not found.";
                return RedirectToAction("MyTickets", "Student");
            }

            // Fetch the assigned user's name
            string assignedToName = null;
            if (ticket.AssignedTo.HasValue)
            {
                // Convert AssignedTo (int?) to string before passing it
                var assignedUser = _userService.GetUserById(ticket.AssignedTo.Value.ToString());
                assignedToName = assignedUser?.Name; // Assuming UserService provides a method to fetch user details
            }

            var model = new TicketViewModel
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                DateCreated = ticket.DateCreated,
                CategoryId = ticket.CategoryId,
                PriorityId = ticket.PriorityId,
                StatusId = ticket.StatusId,
                AttachmentPath = ticket.AttachmentPath,
                AssignedTo = ticket.AssignedTo,
                AssignedToName = assignedToName, // Set the user's name
                Categories = _categoryService.GetAllCategories()
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
                return RedirectToAction("MyTickets", "Student");
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
                _ticketService.DeleteTicket(id);
                return RedirectToAction("MyTickets", "Student");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult EditTicket(int id)
        {
            var ticket = _ticketService.GetTicketById(id);
            if (ticket == null)
            {
                TempData["ErrorMessage"] = "Ticket not found.";
                return RedirectToAction("MyTickets", "Student");
            }

            // Map the ticket data to the ViewModel
            var model = new TicketViewModel
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                CategoryId = ticket.CategoryId,
                PriorityId = ticket.PriorityId,
                StatusId = ticket.StatusId
            };

            return View(model); // Pass the model to the Edit view
        }

        [HttpPost]
        public IActionResult EditTicket(TicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data. Please try again.";
                return RedirectToAction("DetailsTicket", new { id = model.TicketId });
            }

            try
            {
                var ticket = _ticketService.GetTicketById(model.TicketId);
                if (ticket == null)
                {
                    TempData["ErrorMessage"] = "Ticket not found.";
                    return RedirectToAction("MyTickets");
                }

                ticket.Title = model.Title;
                ticket.Description = model.Description;

                _ticketService.UpdateTicket(ticket);

                TempData["SuccessMessage"] = "Ticket updated successfully!";
                return RedirectToAction("DetailsTicket", new { id = model.TicketId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the ticket. Please try again.";
                return RedirectToAction("DetailsTicket", new { id = model.TicketId });
            }
        }




    }
}
