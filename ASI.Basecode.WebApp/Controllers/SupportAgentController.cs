using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult SupportAgentDashboard()
        {
            return View();
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
    }
}
