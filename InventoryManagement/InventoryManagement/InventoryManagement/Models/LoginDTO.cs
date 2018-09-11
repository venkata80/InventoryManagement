using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class LoginDTO
    {
        [DataType(DataType.EmailAddress, ErrorMessage ="Enter valid username.")]
        [Required]
        public string  UserName { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string  Password { get; set; }
        
    }
}