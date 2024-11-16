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

namespace ASI.Basecode.WebApp.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly IUserService _userService;
        public SuperAdminController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult SuperAdminDashboard()
        {
            return View();
        }

        // GET: UsersController
        public ActionResult UserList()
        {
            var users = _userService.GetAllUsers();
            return View(users);
        }

        public ActionResult Tickets()
        {
            return View();
        }

        public ActionResult TicketDetails()
        {
            return View();
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
    }
}
