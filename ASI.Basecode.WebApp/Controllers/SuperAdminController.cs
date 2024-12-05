    using ASI.Basecode.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Data.Entity;
using System.Security.Claims;
using ASI.Basecode.Services.Services;
using ASI.Basecode.Data.Interfaces;

namespace ASI.Basecode.WebApp.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly IPriorityService _priorityService;
        private readonly IStatusService _statusService;
        private readonly ITicketService _ticketService;
        private readonly IFeedbackService _feedbackService;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IUserPreferencesService _userPreferencesService;
        public SuperAdminController(IUserService userService, IUserPreferencesService userPreferencesService, ICategoryService categoryService, IPriorityService priorityService, IStatusService statusService, ITicketService ticketService, IFeedbackRepository feedbackRepository, IFeedbackService feedbackService)
        {
            _userService = userService;
            _categoryService = categoryService;
            _priorityService = priorityService;
            _statusService = statusService;
            _ticketService = ticketService;
            _userPreferencesService = userPreferencesService;
            _feedbackRepository = feedbackRepository;
            _feedbackService = feedbackService;
        }

        public IActionResult SuperAdminDashboard(int? categoryId = null, int? statusId = null, int? priorityId = null)
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

        // GET: UserList
        public IActionResult UserList(string role, string searchId, int page = 1, int pageSize = 10)
        {
            var users = _userService.GetAllUsers().Where(u => u.Role != "Super Admin").ToList();

            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => u.Role == role).ToList();
            }

            if (!string.IsNullOrEmpty(searchId))
            {
                users = users.Where(u => u.UserId.Contains(searchId)).ToList();
                ViewBag.SearchId = searchId; // Preserve the search query in the view
            }

            var totalUsers = users.Count();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var paginatedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new UserListViewModel
            {
                Users = paginatedUsers,
                CurrentPage = page,
                TotalPages = totalPages,
                Role = role
            };

            return View(model);
        }

        public ActionResult Tickets(int? categoryId = null, int? statusId = null, int? priorityId = null, int pageNumber = 1, int pageSize = 5)
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
            var ticketsQuery = _ticketService.GetAllTickets()
                                              .Where(t => (categoryId == 0 || t.CategoryId == appliedCategoryId) &&
                                                         (statusId == 0 || t.StatusId == appliedStatusId) &&
                                                         (priorityId == 0 || t.PriorityId == appliedPriorityId));

            // Pagination
            var totalTickets = _ticketService.GetTicketCount(); // Total tickets based on filters
            var totalPages = (int)Math.Ceiling((double)totalTickets / pageSize); // Calculate total pages

            // Get the tickets for the current page
            var tickets = ticketsQuery.Skip((pageNumber - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToList();

            // Prepare the view model
            var pendingTickets = _ticketService.GetTicketCountByStatus("In Progress");
            var closedTickets = _ticketService.GetTicketCountByStatus("Closed");
            var deletedTickets = _ticketService.GetDeletedTicketCount();

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
                TotalPages = totalPages, // Total pages for pagination
                CurrentPage = pageNumber, // Current page
                PageSize = pageSize // Page size
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
                                            .Where(u => u.Role == "Support Agent")
                                            .Select(u => new { u.UserId, u.Name })
                                            .ToList();

            ViewBag.SupportAgents = new SelectList(supportAgents, "UserId", "Name");
            ViewBag.Ticket = ticket;
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

        // POST: Assign Ticket to an Assignee
        [HttpPost]
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
            if (assignee == null || assignee.Role != "Support Agent")
            {
                TempData["Error"] = "Assignee not found or is not a support agent.";
                return RedirectToAction("TicketDetails", new { id = ticketId });
            }

            ticket.AssignedTo = int.Parse(assignee.UserId);

            _ticketService.UpdateTicket(ticket);

            TempData["Success"] = $"Ticket assigned to {assignee.Name} successfully!";
            return RedirectToAction("Tickets", new { id = ticketId });
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

            return RedirectToAction("Tickets", new { id = TicketId });
        }

        public ActionResult DeleteTicket(int id)
        {
            try
            {
                _ticketService.DeleteTicket(id);
                return RedirectToAction("Tickets", "SuperAdmin");
            }
            catch
            {
                return View();
            }
        }

        // POST: SuperAdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTicket(int id, IFormCollection collection)
        {
            try
            {
                _ticketService.DeleteTicket(id);
                return RedirectToAction("Tickets", "SuperAdmin");
            }
            catch
            {
                return View();
            }
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
                    return RedirectToAction("Settings", "SuperAdmin");
                }
                else
                {
                    return View(model);
                }
            }
            catch
            {
                TempData["Error"] = "An error occurred while changing the password.";
                return RedirectToAction("Password", "SuperAdmin");
            }
        }
    }
}
