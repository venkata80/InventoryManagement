using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Please enter email.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter temporary password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Please enter new password.")]
        [StringLength(100, ErrorMessage = "The new password must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please enter confirm new password.")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirm new password do not match.")]
        [DataType(DataType.Password)]
        public string NewPasswordConfirm { get; set; }

        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }

        public Guid UserID { get; set; }
    }
}