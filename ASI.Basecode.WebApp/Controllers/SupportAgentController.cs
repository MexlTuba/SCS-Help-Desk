using Microsoft.AspNetCore.Mvc;

namespace ASI.Basecode.WebApp.Controllers
{
    public class SupportAgentController : Controller
    {
        public IActionResult SupportAgentDashboard()
        {
            return View();
        }
    }
}
