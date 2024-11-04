using Microsoft.AspNetCore.Mvc;

namespace ASI.Basecode.WebApp.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult StudentDashboard()
        {
            return View();
        }

        public IActionResult CreateTicket()
        {
            return View();
        }

        public IActionResult MyTickets()
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
