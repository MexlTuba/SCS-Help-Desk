using Microsoft.AspNetCore.Mvc;

namespace ASI.Basecode.WebApp.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult StudentDashboard()
        {
            return View();
        }
    }
}
