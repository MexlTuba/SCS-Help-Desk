using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Security.Claims;

namespace ASI.Basecode.WebApp.Controllers
{
    public class SupportAgentController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly IPriorityService _priorityService;
        private readonly IStatusService _statusService;
        private readonly ITicketService _ticketService;
        private readonly IKnowledgebaseService _knowledgebaseService;
        private readonly IUserPreferencesService _userPreferencesService;

        public SupportAgentController(IUserService userService, IUserPreferencesService userPreferencesService, ICategoryService categoryService, IPriorityService priorityService, IStatusService statusService, ITicketService ticketService, IKnowledgebaseService knowledgebaseService)
        {
            _userService = userService;
            _categoryService = categoryService;
            _priorityService = priorityService;
            _statusService = statusService;
            _ticketService = ticketService;
            _userPreferencesService = userPreferencesService;
            _knowledgebaseService = knowledgebaseService;
        }
        public IActionResult SupportAgentDashboard(int? categoryId = null, int? statusId = null, int? priorityId = null)
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
            var totalTickets = _ticketService.GetTicketCount();
            var openTickets = _ticketService.GetTicketCountByStatus("Open");
            var pendingTickets = _ticketService.GetTicketCountByStatus("In Progress");
            var resolvedTickets = _ticketService.GetTicketCountByStatus("Resolved");
            var closedTickets = _ticketService.GetTicketCountByStatus("Closed");
            var deletedTickets = _ticketService.GetTicketCountByStatus("Deleted");

            var enrollmentTickets = _ticketService.GetTicketCountByCategory("Enrollment");
            var gradesTickets = _ticketService.GetTicketCountByCategory("Grades");
            var organizationalTickets = _ticketService.GetTicketCountByCategory("Organizational");
            var inquiryTickets = _ticketService.GetTicketCountByCategory("Inquiry");
            var miscellaneousTickets = _ticketService.GetTicketCountByCategory("Miscellaneous");

            var highTickets = _ticketService.GetTicketCountByPriority("High");
            var mediumTickets = _ticketService.GetTicketCountByPriority("Medium");
            var lowTickets = _ticketService.GetTicketCountByPriority("Low");
            var generalTickets = _ticketService.GetTicketCountByPriority("General");

            var model = new TicketViewModel
            {
                Tickets = tickets,
                Categories = _categoryService.GetAllCategories(),
                Priorities = _priorityService.GetAllPriorities(),
                Statuses = _statusService.GetAllStatuses(),
                CategoryId = categoryId ?? defaultCategoryId, // Preserve selected filter or fallback to user default
                PriorityId = priorityId ?? defaultPriorityId,
                StatusId = statusId ?? defaultStatusId,
                TotalTickets = totalTickets,
                PendingTickets = pendingTickets,
                ClosedTickets = closedTickets,
                DeletedTickets = deletedTickets,
                OpenTickets = openTickets,
                ResolvedTickets = resolvedTickets,
                EnrollmentTickets = enrollmentTickets,
                GradesTickets = gradesTickets,
                OrganizationalTickets = organizationalTickets,
                InquiryTickets = inquiryTickets,
                MiscellaneousTickets = miscellaneousTickets,
                HighTickets = highTickets,
                MediumTickets = mediumTickets,
                LowTickets = lowTickets,
                GeneralTickets = generalTickets

                //Tickets = _ticketService.GetAllTickets(),  //Load statuses for dropdown
            };

            return View(model);
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
            var totalTickets = _ticketService.GetTicketCount();
            var pendingTickets = _ticketService.GetTicketCountByStatus("In Progress");
            var closedTickets = _ticketService.GetTicketCountByStatus("Closed");
            var deletedTickets = _ticketService.GetTicketCountByStatus("Deleted");

            var model = new TicketViewModel
            {
                Tickets = tickets,
                Categories = _categoryService.GetAllCategories(),
                Priorities = _priorityService.GetAllPriorities(),
                Statuses = _statusService.GetAllStatuses(),
                CategoryId = categoryId ?? defaultCategoryId, // Preserve selected filter or fallback to user default
                PriorityId = priorityId ?? defaultPriorityId,
                StatusId = statusId ?? defaultStatusId,
                TotalTickets = totalTickets,
                PendingTickets = pendingTickets,
                ClosedTickets = closedTickets,
                DeletedTickets = deletedTickets,
                //Tickets = _ticketService.GetAllTickets(),  //Load statuses for dropdown
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
                                            .Where(u => u.Role == "SupportAgent")
                                            .Select(u => new { u.UserId, u.Name })
                                            .ToList();

            ViewBag.SupportAgents = new SelectList(supportAgents, "UserId", "Name");
            ViewBag.Ticket = ticket;
            return View();
        }

        public IActionResult AssignTicket(int ticketId, string assigneeId)
        {
            if (ticketId == 0 || string.IsNullOrEmpty(assigneeId))
            {
                TempData["Error"] = "Invalid Ticket or Assignee.";
                return RedirectToAction("Tickets");
            }

            var ticket = _ticketService.GetTicketById(ticketId);
            if (ticket == null)
            {
                TempData["Error"] = "Ticket not found.";
                return RedirectToAction("Tickets");
            }

            var assignee = _userService.GetUserById(assigneeId);
            if (assignee == null || assignee.Role != "SupportAgent")
            {
                TempData["Error"] = "Assignee not found or is not a support agent.";
                return RedirectToAction("TicketDetails", new { id = ticketId });
            }

            ticket.AssignedTo = int.Parse(assignee.UserId);
            if (ticket.StatusId == 1)
            {
                ticket.StatusId = 1;
            }

            _ticketService.UpdateTicket(ticket);

            TempData["Success"] = $"Ticket assigned to {assignee.Name} successfully!";
            return RedirectToAction("TicketDetails", new { id = ticketId });
        }


        [HttpPost]
        public IActionResult UpdateTicket(int TicketId, string AssignedTo)
        {
            var ticket = _ticketService.GetTicketById(TicketId);

            if (ticket != null && !string.IsNullOrEmpty(AssignedTo))
            {
                if (int.TryParse(AssignedTo, out int assignedToId))
                {
                    ticket.AssignedTo = assignedToId;

                    _ticketService.UpdateTicket(ticket);

                    TempData["Success"] = "Ticket assignment updated successfully!";
                }
                else
                {
                    TempData["Error"] = "Invalid assignee selected.";
                }
            }
            else
            {
                TempData["Error"] = "Ticket not found or no assignee provided.";
            }

            return RedirectToAction("TicketDetails", new { id = TicketId });
        }


        //public IActionResult ListArticles(int? categoryId)
        //{
        //    var model = new KnowledgeBaseViewModel
        //    {
        //        Categories = _categoryService.GetAllCategories(),  // Get all categories
        //        Articles = _knowledgebaseService.GetAllArticles()  // Get all articles initially
        //    };

        //    // If categoryId is selected, filter the articles by category
        //    if (categoryId.HasValue && categoryId.Value > 0)
        //    {
        //        model.SelectedCategoryId = categoryId.Value;  // Set the selected category
        //        model.Articles = model.Articles.Where(a => a.CategoryId == categoryId.Value).ToList();  // Filter articles
        //    }

        //    return View(model);
        //}


        public IActionResult ListArticles(int? categoryId)
        {
            var model = new KnowledgeBaseViewModel
            {
                Categories = _categoryService.GetAllCategories() // Get the list of categories
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





        // Create Knowledgebase
        public IActionResult CreateKnowledgebase()
        {
            var model = new KnowledgeBaseViewModel
            {
                Categories = _categoryService.GetAllCategories(),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateKnowledgebase(KnowledgeBaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                _knowledgebaseService.CreateKnowledgebase(new Knowledgebase
                {
                    Title = model.Title,
                    Content = model.Content,
                    CategoryId = model.CategoryId,
                    CreatedBy = User.Identity.Name,
                    CreatedAt = DateTime.Now
                }, User.Identity.Name);
                TempData["SuccessMessage"] = "Article created successfully!";
                return RedirectToAction("ListArticles");
            }
            model.Categories = _categoryService.GetAllCategories();
            return View(model);
        }

        // Edit Knowledgebase
        public IActionResult EditKnowledgebase(int id)
        {
            var article = _knowledgebaseService.GetArticleById(id);
            if (article == null)
            {
                TempData["ErrorMessage"] = "Article not found!";
                return RedirectToAction("ListArticles");
            }
            var model = new KnowledgeBaseViewModel
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Content = article.Content,
                CategoryId = article.CategoryId,
                CategoryName = article.CategoryName,
                CreatedBy = article.CreatedBy,
                CreatedAt = article.CreatedAt,
                Categories = _categoryService.GetAllCategories()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditKnowledgebase(KnowledgeBaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                _knowledgebaseService.UpdateKnowledgebase(model, User.Identity.Name);
                TempData["SuccessMessage"] = "Article updated successfully!";
                return RedirectToAction("ListArticles");
            }
            model.Categories = _categoryService.GetAllCategories();
            return View(model);
        }

        // Delete Knowledgebase
        public IActionResult DeleteKnowledgebase(int id)
        {
            // Assuming the service method returns a KnowledgeBaseModel
            var article = _knowledgebaseService.GetArticleById(id);

            if (article == null)
            {
                TempData["ErrorMessage"] = "Article not found.";
                return RedirectToAction("ListArticles");
            }

            // Map the KnowledgeBaseModel to KnowledgeBaseViewModel
            var articleViewModel = new KnowledgeBaseViewModel
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Content = article.Content,
                CategoryName = article.CategoryName,
                CreatedBy = article.CreatedBy,
                CreatedAt = article.CreatedAt
            };

            return View(articleViewModel);  // Pass the ViewModel to the view
        }


        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            // Call service to delete the article
            _knowledgebaseService.DeleteKnowledgebase(id);

            // Optionally, set a success message
            TempData["SuccessMessage"] = "Article deleted successfully!";

            // Redirect back to ListArticles page
            return RedirectToAction("ListArticles");
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

            return View(model);
        
            try
            {
                if (model.Password == model.ConfirmPassword)
                {
                    _userService.ResetPassword(model.UserId, model.Password);
                    return RedirectToAction("Settings", "SupportAgent");
                }
                else
                {
                    return View(model);
                }
            }
            catch
            {
                TempData["Error"] = "An error occurred while changing the password.";
                return RedirectToAction("Password", "SupportAgent");
            }
        }
    }
}
