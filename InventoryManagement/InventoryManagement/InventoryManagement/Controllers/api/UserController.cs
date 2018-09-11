using InventoryManagement.EF;
using InventoryManagement.Enums;
using InventoryManagement.Models;
using InventoryManagement.Security;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Security;

namespace InventoryManagement.Controllers.api
{
    public class UserController : ApiController
    {
        [HttpGet]
        [ActionName("GetMemberById")]
        public IHttpActionResult GetMember()
        {
            MemberShipDTO Member = new MemberShipDTO();

            //using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            //{
            //    students = hhh.IM_Employer.Select(s => new BaseEmployerDTO()
            //    {
            //        Id = s.EMPID,
            //        LastName = s.LastName,
            //        FirstName = s.FirstName,
            //        MiddleName = s.Middlename,
            //        //Fullname=s.FirstName+ ' '+s.Middlename+' '+s.LastName,
            //        Designation = s.Designation,
            //        Dateofbirth = s.DOB,
            //        gender = s.Gender,
            //        Email = s.Email,
            //        ResPhone = s.ResPhone,
            //        CellPhone = s.CellPhone,
            //        Address = s.Address,
            //        City = s.City,
            //        State = s.State,
            //        Zipcode = s.Zipcode,
            //        JoinDate = s.JoinDate,
            //        Relieved = s.RelievedDate,
            //        Isactive = true
            //    }).ToList<BaseEmployerDTO>();
            //    if (students == null)
            //        students = new List<BaseEmployerDTO>();
            //}
            //if (students.Count == 0)
            //    return NotFound();
            return Ok(Member);
        }

        [HttpPost]
        public UserDTO CreateUser(UserDTO user)
        {
            using (var ctx = new InventoryManagementEntities())
            {
                MembershipUser mUser = null;
                try
                {
                    MembershipCreateStatus status = MembershipCreateStatus.Success;
                    Guid newUserID = Guid.NewGuid();
                    //get the requirements from config file to generate a temporary password:
                    MembershipSection membershipSection = (MembershipSection)WebConfigurationManager.GetSection("system.web/membership");
                    ProviderSettings providerSettings = membershipSection.Providers[membershipSection.DefaultProvider];
                    string minPwdReqLength = providerSettings.Parameters["minRequiredPasswordLength"];
                    string minAlphaNumericChars = providerSettings.Parameters["minRequiredNonalphanumericCharacters"];
                    string password=  Membership.GeneratePassword(Convert.ToInt32(minPwdReqLength), Convert.ToInt32(minAlphaNumericChars));

                    mUser = Membership.CreateUser(user.Email.ToLower(), password, user.Email, "hi", "bye", true, newUserID, out status);

                    switch (status)
                    {
                        case MembershipCreateStatus.DuplicateUserName:
                            {
                                throw new ApplicationException("This email address already exists. Please log in or register using a different email address.");
                            }
                        case MembershipCreateStatus.InvalidEmail:
                            {
                                throw new ApplicationException("Email entered is invalid. Please try again");
                            }
                        case MembershipCreateStatus.DuplicateEmail:
                            {
                                throw new ApplicationException("Email already exists. Please try again");
                            }
                        case MembershipCreateStatus.InvalidQuestion:
                            {
                                throw new ApplicationException("Invalid question. Please try a different question");
                            }
                        case MembershipCreateStatus.InvalidAnswer:
                            {
                                throw new ApplicationException("Invalid answer. Please try a different answer");
                            }
                        case MembershipCreateStatus.Success:
                            {
                                if (mUser != null)
                                {
                                    //Assign Role
                                    string[] roles = new string[1];
                                    roles[0] = user.Role.Name;
                                    string[] users = new string[1];
                                    users[0] = user.Email;
                                    Roles.AddUsersToRoles(users, roles);

                                    user.Id = newUserID;

                                    ctx.Users.Add(new EF.User()
                                    {
                                        ID = user.Id,
                                        FirstName = user.FirstName,
                                        LastName = user.LastName,
                                        MiddleName = user.MiddleName,
                                        Email = user.Email,
                                        ActiveFL = user.Isactive,
                                        CreatedBy = user.Id,
                                        ModifiedBy = user.Id,
                                        CreatedDate = DateTime.UtcNow
                                    });

                                    ctx.SaveChanges();
                                }
                                break;
                            }

                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {

                }
            }
            return user;
        }

        [HttpGet]
        public IHttpActionResult ValidateUser(string username, string password)
        {
            try
            {
                UserAccountStatus result = UserAccountStatus.NotSet;
                EFMembershipProvider membershipprovider = new EFMembershipProvider();
                membershipprovider.Initialize("SqlProvider", new NameValueCollection());

                MembershipUser user = membershipprovider.GetUser(username, false);
                DateTime lastLoginDate = DateTime.MinValue;
                if (user != null)
                {
                    //Get the last login date
                    lastLoginDate = user.LastLoginDate;
                }

                if (membershipprovider.ValidateUser(username, password))
                {
                    using (var ctx = new InventoryManagementEntities())
                    {
                        //check if the employer, user is with, is active:
                        if (ctx.Users != null && ctx.Users.Any() && ctx.Users.Any(c => c.Email == user.UserName && c.ActiveFL == true) == false)
                        {
                            result = UserAccountStatus.InactiveEmployer;
                            return Ok(result);
                        }
                        //Update user table with last login date
                        if (user != null)
                            if (user.ProviderUserKey != null)
                            {
                                Guid.TryParse(user.ProviderUserKey.ToString(), out Guid userid);
                                User loginUser = ctx.Users.FirstOrDefault(c => c.ID == userid);
                                if (loginUser != null)
                                {
                                    loginUser.LastLoginDate = lastLoginDate;
                                    ctx.SaveChanges();
                                }
                            }
                    }
                    result = UserAccountStatus.Success;
                    return Ok(result);
                }
                if (user != null)
                {
                    //user is locked out due to too many incorrect login attempts:
                    if (user.IsLockedOut)
                        return Ok(UserAccountStatus.LockedOut);

                    //user account is inactive:
                    return Ok(!user.IsApproved ? UserAccountStatus.Inactive : UserAccountStatus.InvalidCredentials);
                }
                result = UserAccountStatus.AccountNotFound;
                return Ok(result);
            }
            catch (Exception e)
            {
                throw new Exception("Error validating user", e);
            }
        }

        [HttpGet]
        public IHttpActionResult GetUserByEmail(string email)
        {
            UserSecurityToken userdetails = null;
            using (var ctx = new InventoryManagementEntities())
            {
                User user = ctx.Users.FirstOrDefault(c => c.Email == email);
                if (user != null)
                {
                    userdetails = new UserSecurityToken
                    {
                        Id = user.ID,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        MiddleName = user.MiddleName,
                        Email = user.Email,
                        Role = RoleType.GetRoleType(ctx.aspnet_UsersInRoles_GetRolesForUser(user.aspnet_Users.aspnet_Applications.ApplicationName, user.Email)?.FirstOrDefault())
                    };
                }
            }
            return Ok(userdetails);
        }

        [HttpGet]
        public IHttpActionResult ChangePassword(string userName, string currentPassword, string newPassword, string confirmPassword)
        {
            bool status = false;

            try
            {
                MembershipUser mUser = Membership.GetUser(userName, false);
                if (newPassword != confirmPassword)
                {
                    throw new ApplicationException("New Password and Confirm Password dont match. Please try again");
                }
                if (mUser.ChangePassword(mUser.GetPassword(), newPassword))
                {
                    //new BLCommunication().SendChangePasswordEmail((int)mUser.ProviderUserKey);
                    using (InventoryManagementEntities changepwdentity=new InventoryManagementEntities())
                    {
                        User user= changepwdentity.Users.FirstOrDefault(c => c.Email.ToLower() == userName.ToLower());
                        if (user != null)
                        {
                            string RegistrationTemplate = Resources.Communication.MailTemplates.ChangePassword_Body;
                            if (!string.IsNullOrWhiteSpace(RegistrationTemplate))
                            {
                                RegistrationTemplate = RegistrationTemplate.Replace("[[[Name]]]", string.Concat(user.FirstName, " ", user.LastName));
                                MailMessage mailMessage = new MailMessage();
                                mailMessage.To.Add(new MailAddress(user.Email));
                                mailMessage.Subject = Resources.Communication.MailTemplates.ChangePassword_Subject;
                                mailMessage.Body = RegistrationTemplate;
                                mailMessage.From = new MailAddress(Resources.Communication.MailTemplates.SmtpClient_UserName);
                                mailMessage.IsBodyHtml = true;

                                SendMail(mailMessage);
                            }
                        }
                    }
                    
                    status = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error changing password", e);
            }
            return Ok(status);
        }

        [HttpGet]
        public IHttpActionResult ForgotPassword(string username)
        {
            //get user account:
            MembershipUser mUser = Membership.GetUser(username);
            if (mUser != null && mUser.IsApproved && !mUser.IsLockedOut)
            {
                //new BLCommunication().SendResetPasswordEmail((int)mUser.ProviderUserKey);
                using (InventoryManagementEntities changepwdentity = new InventoryManagementEntities())
                {
                    User user = changepwdentity.Users.FirstOrDefault(c => c.Email.ToLower() == username.ToLower());
                    if (user != null)
                    {
                        string RegistrationTemplate = Resources.Communication.MailTemplates.ForgotPassword_Body;
                        if (!string.IsNullOrWhiteSpace(RegistrationTemplate))
                        {
                            RegistrationTemplate = RegistrationTemplate.Replace("[[[Name]]]", string.Concat(user.FirstName, " ", user.LastName));
                            RegistrationTemplate = RegistrationTemplate.Replace("[[[Link]]]", string.Concat(Resources.Communication.MailTemplates.LoginLink, Resources.Communication.MailTemplates.ResetPasswordLink));
                            RegistrationTemplate = RegistrationTemplate.Replace("[[[UserID]]]", user.ID.ToString());
                            MailMessage mailMessage = new MailMessage();
                            mailMessage.To.Add(new MailAddress(user.Email));
                            mailMessage.Subject = Resources.Communication.MailTemplates.ForgotPassword_Subject;
                            mailMessage.Body = RegistrationTemplate;
                            mailMessage.From = new MailAddress(Resources.Communication.MailTemplates.SmtpClient_UserName);
                            mailMessage.IsBodyHtml = true;

                            SendMail(mailMessage);
                        }
                    }
                }
                return Ok(mUser.ProviderUserKey);
            }

            return Ok(Int32.MinValue);
        }

        [HttpGet]
        public IHttpActionResult ResetPassword(Guid userid, string newPassword)
        {
            try
            {
                MembershipUser user = Membership.GetUser(userid);
                if (user == null)
                    return Ok(UserAccountStatus.AccountNotFound);

                //change password:
                if (user.ChangePassword(user.GetPassword(), newPassword))
                    return Ok(UserAccountStatus.Success);
            }
            catch (Exception e)
            {
                throw new Exception("Error resetting password", e);
            }
            return Ok(UserAccountStatus.Unsupported);
        }

        void SendMail(MailMessage Message)
        {
            SmtpClient client = new SmtpClient();
            client.Host = Resources.Communication.MailTemplates.SmtpClient_Host;
            client.Port = Convert.ToInt32(Resources.Communication.MailTemplates.SmtpClient_Port);
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(Resources.Communication.MailTemplates.SmtpClient_UserName, Resources.Communication.MailTemplates.SmtpClient_Password);
            client.Send(Message);
        }

    }
}
