using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class ApplicationDTO
    {
        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationDesc { get; set; }
        public string LoweredApplicationName { get; set; }
    }
    public class UsersInRolesDTO
    {
        public string ApplicationId { get; set; }
        public string RoleId { get; set; }       
    }
    public class RoleDTO
    {
        public string ApplicationId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string LoweredRoleName { get; set; }
        public string RoleDesc { get; set; }
    }
    public class UserDTO
    {
        public string ApplicationId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string LoweredUserName { get; set; }
        public string MobileAlias { get; set; }
        public bool IsAnonymous { get; set; }       
        public DateTime? LastActivityDate { get; set; }
    }
    public class MemberShipDTO
    {
        public string ApplicationId { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public int PasswordFormat { get; set; }
        public string PasswordSalt { get; set; }
        public string MobilePIN { get; set; }
        public string Email { get; set; }
        public string LoweredEmail { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public DateTime? FailedPasswordAttemptWindowStart { get; set; }
        public int FailedPasswordAnswerAttemptCount { get; set; }
        public DateTime? FailedPasswordAnswerAttemptWindowStart { get; set; }
        public string Comment { get; set; }
    }
}