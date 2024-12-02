using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class SupportAgentController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly IPriorityService _priorityService;
        private readonly IStatusService _statusService;
        private readonly ITicketService _ticketService;

        public SupportAgentController(IUserService userService, ICategoryService categoryService, IPriorityService priorityService, IStatusService statusService, ITicketService ticketService)
        {
            _userService = userService;
            _categoryService = categoryService;
            _priorityService = priorityService;
            _statusService = statusService;
            _ticketService = ticketService;
        }

        // GET: SupportAgent Dashboard
        public IActionResult SupportAgentDashboard()
        {
            return View();
        }

        // GET: List of Tickets
        public ActionResult Tickets()
        {
            var model = new TicketViewModel
            {
                Tickets = _ticketService.GetAllTickets(),
                Categories = _categoryService.GetAllCategories(),  // Load categories for dropdown
                Priorities = _priorityService.GetAllPriorities(),  // Load priorities for dropdown
                Statuses = _statusService.GetAllStatuses()    // Load statuses for dropdown
            };
            return View(model);
        }

        // GET: Ticket Details
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

            var statuses = _statusService.GetAllStatuses();
            if (statuses != null && statuses.Any())
            {
                ViewBag.StatusList = new SelectList(statuses, "StatusId", "StatusType");
            }

            var priorities = _priorityService.GetAllPriorities();
            if (priorities != null && priorities.Any())
            {
                ViewBag.PriorityList = new SelectList(priorities, "ProirityId", "PriorityType");
            }

            ViewBag.Ticket = ticket;
            return View();
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
            if (assignee == null || assignee.Role != "SupportAgent")
            {
                TempData["Error"] = "Assignee not found or is not a support agent.";
                return RedirectToAction("TicketDetails", new { id = ticketId });
            }

            // Assuming assignee.UserId is an integer
            if (int.TryParse(assignee.UserId, out int userId))
            {
                ticket.AssignedTo = userId;
            }
            else
            {
                TempData["Error"] = "Invalid UserId.";
                return RedirectToAction("TicketDetails", new { id = ticketId });
            }

            if (ticket.StatusId == 1) // Assuming 1 is the default status
            {
                ticket.StatusId = 1;
            }

            _ticketService.UpdateTicket(ticket);

            TempData["Success"] = $"Ticket assigned to {assignee.Name} successfully!";
            return RedirectToAction("TicketDetails", new { id = ticketId });
        }

        // POST: Update Ticket Details (Priority, Status, Assignee)
        [HttpPost]
        public IActionResult UpdateTicket(int ticketId, int? assignedTo, int? statusId, int? priorityId)
        {
            // Retrieve the ticket by ID
            var ticket = _ticketService.GetTicketById(ticketId);
            if (ticket != null)
            {
                // Update ticket details
                if (assignedTo.HasValue)
                    ticket.AssignedTo = assignedTo.Value;

                if (statusId.HasValue)
                    ticket.StatusId = statusId.Value;

                if (priorityId.HasValue)
                    ticket.PriorityId = priorityId.Value;

                // Save changes
                _ticketService.UpdateTicket(ticket);
            }

            return RedirectToAction("Tickets", "SupportAgent");
        }
    }
}
