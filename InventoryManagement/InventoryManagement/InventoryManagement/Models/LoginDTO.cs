using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class LoginDTO
    {
        public int LoginId { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage ="Enter valid username.")]
        [Required]
        public string  UserName { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string  Password { get; set; }
        public int LoginAttempts { get; set; }
        public bool ResetPassword { get; set; }
        public int EmployerId { get; set; }
        [Required]
        public int Role { get; set; }
        public bool Isactive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}