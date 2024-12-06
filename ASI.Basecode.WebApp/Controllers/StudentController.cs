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
using System.Security.Claims;
using System.Security.AccessControl;

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
        private readonly IFeedbackService _feedbackService;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IKnowledgebaseService _knowledgebaseService;

        public StudentController(ITicketService ticketService, ITicketRepository ticketRepository, ICategoryService categoryService, IUserPreferencesService userPreferencesService, IUserService userService, IPriorityService priorityService, IStatusService statusService, IFeedbackRepository feedbackRepository, IFeedbackService feedbackService, IKnowledgebaseService knowledgebaseService)
        {
            _ticketRepository = ticketRepository;
            _categoryService = categoryService;
            _priorityService = priorityService;
            _statusService = statusService;
            _userService = userService;
            _userPreferencesService = userPreferencesService;
            _ticketService = ticketService;
            _knowledgebaseService = knowledgebaseService;
            _feedbackRepository = feedbackRepository;
            _feedbackService = feedbackService;
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

            var feedback = _feedbackService.GetFeedbackByTicketId(id);
            var feedbackExists = _feedbackService.GetFeedbackByTicketId(id) != null;

            var model = new DetailsTicketViewModel
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                DateCreated = ticket.DateCreated,
                HasFeedback = feedbackExists,
                AttachmentPath = ticket.AttachmentPath,
                CategoryId = ticket.CategoryId,
                PriorityId = ticket.PriorityId,
                StatusId = ticket.StatusId,
                AssignedToName = ticket.AssignedTo.HasValue
                    ? _userService.GetUserById(ticket.AssignedTo.Value.ToString())?.Name
                    : "Unassigned",
                TicketRating = feedback?.TicketRating,
                TicketComment = feedback?.TicketComment,
                AgentRating = feedback?.AgentRating,
                AgentComment = feedback?.AgentComment,
                UserId = feedback?.UserId,
                Categories = _categoryService.GetAllCategories().ToList(),
                Priorities = _priorityService.GetAllPriorities().ToList(),
                Statuses = _statusService.GetAllStatuses().ToList()
            };

            return View(model);
        }




        public IActionResult StudentDashboard()
        {
            return View();
        }

        public IActionResult ListArticles(int? categoryId)
        {
            var model = new KnowledgeBaseViewModel
            {
                Categories = _categoryService.GetAllCategories()
            };

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                // Get filtered articles by category
                model.Articles = _knowledgebaseService.GetArticlesByCategory(categoryId.Value);
            }
            else
            {
                // No category filter applied, show all articles
                model.Articles = _knowledgebaseService.GetAllArticles();
            }

            return View(model);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFeedback(FeedbackViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Invalid feedback. Please check your inputs.";
                    return RedirectToAction("DetailsTicket", new { id = model.TicketId });
                }

                var userName = User.Identity.Name;
                Console.WriteLine($"Feedback TicketId: {model.TicketId}");


              
                _feedbackService.AddFeedback(model, userName);

                TempData["SuccessMessage"] = "Feedback submitted successfully!";

                return RedirectToAction("DetailsTicket", new { id = model.TicketId });
            }
            catch (Exception ex)
            {
                
                TempData["ErrorMessage"] = $"An error occurred while submitting feedback: {ex}";
               
                Console.WriteLine($"Error in AddFeedback: {ex.Message}");

                return RedirectToAction("DetailsTicket", new { id = model.TicketId });
            }
        }


        public IActionResult Password()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve the logged-in user's ID
            var model = new UserViewModel
            {
                UserId = userId
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(UserViewModel model)
        {

            try
            {
                if (model.Password == model.ConfirmPassword)
                {
                    _userService.ResetPassword(model.UserId, model.Password);
                    return RedirectToAction("Settings", "Student");
                }
                else
                {
                    return View(model);
                }
            }
            catch
            {
                TempData["Error"] = "An error occurred while changing the password.";
                return RedirectToAction("Password", "Student");
            }
        }


    }
}
