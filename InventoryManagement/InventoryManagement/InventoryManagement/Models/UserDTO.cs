using InventoryManagement.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class UserDTO : CommonBaseDTO<Guid>
    {
        
        [Display(Name = "First Name*")]
        [Required(ErrorMessage = "Enter first name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter last name.")]
        [Display(Name = "Last Name*")]
        public string LastName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        public string Fullname { get { return string.Concat(FirstName, " ", LastName); } }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Residential Phone")]
        public int? ResPhone { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Cell Phone")]
        public int? CellPhone { get; set; }

        [Display(Name = "Email*")]
        [Required(ErrorMessage = "Enter email.")]
        public string Email { get; set; }

        [Display(Name = "Is Active")]
        public bool Isactive { get; set; }

        public RoleType Role { get; set; }
    }
}