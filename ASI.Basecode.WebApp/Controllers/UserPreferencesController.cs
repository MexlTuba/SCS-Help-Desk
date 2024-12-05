using ASI.Basecode.Data.Models;
using ASI.Basecode.Services;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;


namespace ASI.Basecode.WebApp.Controllers
{
    public class UserPreferencesController : Controller
    {
        private readonly IUserPreferencesService _preferencesService;
        private readonly IUserService _userService;

        public UserPreferencesController(IUserPreferencesService preferencesService, IUserService userService)
        {
            _preferencesService = preferencesService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Edit(string userId)
        {
            // Retrieve current preferences for the user
            var preferences = _preferencesService.GetPreferencesByUserId(userId);
            if (preferences == null)
            {
                return NotFound("User preferences not found.");
            }

            // Map the entity to the ViewModel
            var model = new UserPreferencesViewModel
            {
                UserId = preferences.UserId,
                DefaultCategoryId = preferences.DefaultCategoryId,
                DefaultStatusId = preferences.DefaultStatusId,
                DefaultPriorityId = preferences.DefaultPriorityId
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(UserPreferencesViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Map the view model to the UserPreferences entity
                var preferences = new UserPreferences
                {
                    UserId = model.UserId,
                    DefaultCategoryId = model.DefaultCategoryId,
                    DefaultStatusId = model.DefaultStatusId,
                    DefaultPriorityId = model.DefaultPriorityId,
                    UpdatedTime = DateTime.UtcNow
                };

                // Update preferences using the service
                _preferencesService.UpdatePreferences(preferences);

                TempData["SuccessMessage"] = "Preferences updated successfully.";
                // Fetch user's role from the service or session
                var user = _userService.GetUserById(model.UserId);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account"); // Fallback to login
                }

                // Redirect based on role
                return RedirectToRoleBasedDashboard(user.Role);
            }

            return View(model);
        }
        private IActionResult RedirectToRoleBasedDashboard(string role)
        {
            switch (role)
            {
                case "Super Admin":
                    return RedirectToAction("Tickets", "SuperAdmin");
                case "Admin":
                    return RedirectToAction("Tickets", "Admin");
                case "Support Agent":
                    return RedirectToAction("Tickets", "SupportAgent");
                case "Student":
                    return RedirectToAction("MyTickets", "Student");
                default:
                    return RedirectToAction("Login", "Account"); // Fallback for unexpected roles
            }
        }
    }
}
