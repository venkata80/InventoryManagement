using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using System.Web.Security;
namespace InventoryManagement.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult UserLogin()
        {
            ViewBag.Title = "Login Page";
            LoginDTO loginDetails = new LoginDTO();
            return View(loginDetails);
        }
        [HttpPost]
        public ActionResult UserLogin(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "");
                return View(model);
            }
            if(model.UserName==model.Password)
                return RedirectToAction("Index","Home");
            return View(model);
        }

        public ActionResult CreateLogin()
        {
            LoginDTO newlogin = new LoginDTO();
            return View(newlogin);
        }


    }
}