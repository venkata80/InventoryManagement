using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using System.Web.Security;
using InventoryManagement.Enums;
using InventoryManagement.Controllers.api;
using System.Net.Http;

namespace InventoryManagement.Controllers
{
    public class AccountController : Controller
    {
        string value = System.Configuration.ConfigurationManager.AppSettings["urlname"];
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
            if (ModelState.IsValid)
            {
                UserAccountStatus loginStatus = UserAccountStatus.NotSet;
                
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(value);
                    
                    var validateUserTask = client.GetAsync("User/ValidateUser?username=" + model.UserName + "&password=" + model.Password);
                    validateUserTask.Wait();

                    HttpResponseMessage result = validateUserTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readUserAccountStatusTask = result.Content.ReadAsAsync<UserAccountStatus>();
                        readUserAccountStatusTask.Wait();
                        loginStatus = readUserAccountStatusTask.Result;
                        switch (loginStatus)
                        {

                            case UserAccountStatus.InvalidCredentials:
                                ModelState.AddModelError("Password", "Incorrect username or password, please try again.");
                                break;
                            case UserAccountStatus.LockedOut:
                                ModelState.AddModelError("Password", "Your account has been locked out because of too many failed login attempts. Please contact the administrator to have your account unlocked.");
                                break;
                            case UserAccountStatus.Inactive:
                                ModelState.AddModelError("Password", "Your account is currently inactive, please contact your administrator for any further questions regarding this account.");
                                break;
                            case UserAccountStatus.InactiveEmployer:
                                ModelState.AddModelError("Password", "Your employer account is currently inactive, please contact your administrator for any further questions regarding this account.");
                                break;
                            case UserAccountStatus.AccountNotFound:
                                ModelState.AddModelError("UserName", "Incorrect username or password, please try again.");
                                break;
                            case UserAccountStatus.Success:
                                {
                                    result = null;
                                    var userinfo = client.GetAsync("User/GetUserByEmail?email=" + model.UserName);
                                    userinfo.Wait();
                                    result = userinfo.Result;
                                    if (result.IsSuccessStatusCode)
                                    {
                                        var readUserDTOTask = result.Content.ReadAsAsync<UserSecurityToken>();
                                        readUserDTOTask.Wait();

                                        UserSecurityToken userSecurityToken = readUserDTOTask.Result;
                                        Session["CurrentUser"] = userSecurityToken;
                                        if (userSecurityToken != null)
                                        {
                                            if (userSecurityToken.Role.Guid == RoleType.Admin.Guid)
                                                return RedirectToAction("Index", "Admin");
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
            else
            {

            }
            return View(model);
        }

        public ActionResult CreateLogin()
        {
            LoginDTO newlogin = new LoginDTO();
            return View(newlogin);
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("UserLogin");
        }

    }
}