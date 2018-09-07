using InventoryManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class EmployerDTO : UserDTO
    {
        public EmployerDTO()
        {
            base.Role = RoleType.EmployerAdmin;
        }
        public string Designation { get; set; }

        [Display(Name = "Gender*")]
        public string gender { get; set; } = "Male";

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Enter date of birth.")]
        [Display(Name = "Date Of Birth*")]
        public DateTime? Dateofbirth { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Enter Join date.")]
        [Display(Name = "Joining Date*")]
        public DateTime? JoinDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Relieved Date")]
        public DateTime? Relieved { get; set; }
        public Guid? AddressID { get; set; }
        public string Dboperation { get; set; }

        public AddressDTO Address { get; set; }

    }
}