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
                                        if (Session["MasterData"] == null)
                                            Session["MasterData"] =new AdminController().ReadMasterData(MasterDataType.None);
                                        if (Session["SupplierList"] == null)
                                            Session["SupplierList"] = new AdminController().SupplierPriceList();

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

        public ActionResult ChangePassword()
        {
            if (Session["CurrentUser"] != null)
            {
                ForgotPasswordModel password = new ForgotPasswordModel();
                return View(password);
            }
            return RedirectToAction("UserLogin", "Account");
        }

        [HttpPost]
        public ActionResult ChangePassword(ForgotPasswordModel password)
        {
            try
            {
                if (Session["CurrentUser"] != null)
                {
                    UserSecurityToken currentUser = (UserSecurityToken)Session["CurrentUser"];
                    password.UserName = currentUser.Email;


                    if (ModelState.ContainsKey("UserName"))
                        ModelState.Remove("UserName");

                    if (ModelState.ContainsKey("Password"))
                        ModelState.Remove("Password");

                    if (ModelState.ContainsKey("UserID"))
                        ModelState.Remove("UserID");

                    if (ModelState.IsValid)
                    {
                        bool success = false;
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(value);
                            var responseTask1 = client.GetAsync(string.Format("User/ChangePassword?userName={0}&currentPassword={1}&newPassword={2}&confirmPassword={3}", password.UserName, password.OldPassword, password.NewPassword, password.NewPasswordConfirm));
                            responseTask1.Wait();
                            var result = responseTask1.Result;

                            if (result.IsSuccessStatusCode)
                            {
                                var changepasswordTask = result.Content.ReadAsAsync<bool>();
                                changepasswordTask.Wait();

                                success = changepasswordTask.Result;
                                if (success)
                                {
                                    switch (currentUser.Role.Name)
                                    {
                                        case "Admin":
                                            return RedirectToAction("Index", "Admin");

                                    }
                                }
                            }
                            ModelState.AddModelError("NewPassword", "Password was not changed. Please try again.");
                            return View(password);
                        }
                    }
                    return View(password);
                }
                return RedirectToAction("UserLogin", "Account");
            }
            catch
            {
                ModelState.AddModelError("NewPassword", "New password must be at least 8 characters long. Please try again.");
                return View(password);
            }
        }

        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordModel());
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel forgotPassword)
        {
            UserDTO user = null;
            try
            {
                if (ModelState.ContainsKey("UserID"))
                    ModelState.Remove("UserID");

                if (ModelState.ContainsKey("Password"))
                    ModelState.Remove("Password");

                if (ModelState.ContainsKey("NewPassword"))
                    ModelState.Remove("NewPassword");

                if (ModelState.ContainsKey("NewPasswordConfirm"))
                    ModelState.Remove("NewPasswordConfirm");

                if (!string.IsNullOrWhiteSpace(forgotPassword.UserName))
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(value);
                        var chekcusernameexists = client.GetAsync("User/GetUserByEmail?email=" + forgotPassword.UserName);
                        chekcusernameexists.Wait();
                        var result = chekcusernameexists.Result;

                        if (result.IsSuccessStatusCode)
                        {
                            var readUserDTOTask = result.Content.ReadAsAsync<UserDTO>();
                            readUserDTOTask.Wait();

                            user = readUserDTOTask.Result;
                            if (user != null)
                            {
                                var forgotpasswordrequest = client.GetAsync("User/ForgotPassword?username=" + forgotPassword.UserName);
                                forgotpasswordrequest.Wait();
                                var forgotpasswordresult = forgotpasswordrequest.Result;

                                if (forgotpasswordresult.IsSuccessStatusCode)
                                {
                                    var readforgotpasswordresultTask = result.Content.ReadAsAsync<int>();
                                    readforgotpasswordresultTask.Wait();

                                    int taskresult = readforgotpasswordresultTask.Result;
                                    if (taskresult != Int32.MinValue)
                                    {
                                        return RedirectToAction("UserLogin", "Account");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            ModelState.AddModelError("UserName", "Please check the email address entered and try again.");
            return View(forgotPassword);
        }

        public ViewResult ResetPassword(string userid)
        {
            Guid.TryParse(Convert.ToString(userid), out Guid resultid);
            return View(new ForgotPasswordModel() { UserID = resultid });
        }

        [HttpPost]
        public ActionResult ResetPassword(ForgotPasswordModel resetpassword)
        {
            try
            {
                if (ModelState.ContainsKey("Password"))
                    ModelState.Remove("Password");

                if (ModelState.ContainsKey("UserName"))
                    ModelState.Remove("UserName");

                if (ModelState.IsValid)
                {
                    UserAccountStatus status = UserAccountStatus.NotSet;

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(value);
                        var resetpasswordrequest = client.GetAsync(string.Format("User/ResetPassword?userid={0}&newPassword={1}", resetpassword.UserID, resetpassword.NewPassword));
                        resetpasswordrequest.Wait();
                        var resetpasswordresult = resetpasswordrequest.Result;

                        if (resetpasswordresult.IsSuccessStatusCode)
                        {
                            var resetpasswordtask = resetpasswordresult.Content.ReadAsAsync<UserAccountStatus>();
                            resetpasswordtask.Wait();

                            status = resetpasswordtask.Result;
                            switch (status)
                            {
                                case UserAccountStatus.AccountNotFound:
                                    ModelState.AddModelError("UserName", "No matching account could be found. Please try again.");
                                    resetpassword.NewPassword = string.Empty;
                                    resetpassword.NewPasswordConfirm = string.Empty;
                                    return View(resetpassword);
                                case UserAccountStatus.UsernameMismatch:
                                    ModelState.AddModelError("UserName", "User Name is incorrect. Please try again.");
                                    resetpassword.NewPassword = string.Empty;
                                    resetpassword.NewPasswordConfirm = string.Empty;
                                    return View(resetpassword);
                                case UserAccountStatus.Success:
                                    return RedirectToAction("UserLogin", "Account");
                            }
                        }
                    }
                }
            }
            catch
            {
                ModelState.AddModelError("EmailAddress", "Password has not been reset. Please check the email address entered and try again.");
            }
            return View(resetpassword);
        }
    }
}