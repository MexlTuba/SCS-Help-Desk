using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using ASI.Basecode.Services.Interfaces;

namespace ASI.Basecode.WebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        // GET: UsersController
        public ActionResult UserList()
        {
            return View();
        }

        // GET: UsersController/UserAdd
        public ActionResult UserAdd()
        {
            return View();
        }

        // GET: UsersController/Edit/UserId
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsersController/Edit/userId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/userId
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsersController/Delete/userId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
