using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public abstract class BaseInformation
    {
        //public int Id { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public string MiddleName { get; set; }
        //public string Fullname { get; set; }
        //public int? Phonenumber { get; set; }
        //public string Address { get; set; }
        //public DateTime? JoinDate { get; set; }
        //public DateTime? Relieved { get; set; }
        //public bool? Isactive { get; set; }
        //public string Dboperation { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime? Createddate { get; set; }
        //public string ModifiedBy { get; set; }
        //public DateTime? ModifiedDate { get; set; }
        //public AddressDTO AddressDTO { get; set; }
        public int Id { get; set; }

        [Display(Name = "First Name*")]
        [Required(ErrorMessage = "Enter first name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter last name.")]
        [Display(Name = "Last Name*")]
        public string LastName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        public string Fullname { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Enter date of birth.")]
        [Display(Name = "Date Of Birth*")]
        public DateTime? Dateofbirth { get; set; }

        [Display(Name = "Gender*")]
        public string gender { get; set; } = "Male";

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Residential Phone")]
        public int? ResPhone { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Cell Phone")]
        public int? CellPhone { get; set; }

        [Display(Name = "Email*")]
        [Required(ErrorMessage = "Enter email.")]
        public string Email { get; set; }

        [Display(Name = "Address*")]
        [Required(ErrorMessage = "Enter address.")]
        public string Address { get; set; }

        [Display(Name = "City*")]
        [Required(ErrorMessage = "Enter city.")]
        public string City { get; set; }

        [Display(Name = "State*")]
        [Required(ErrorMessage = "Enter state.")]
        public string State { get; set; }

        [DataType(DataType.PostalCode)]
        [Required(ErrorMessage = "Enter zip code.")]
        [Display(Name = "Zip Code*")]
        public int? Zipcode { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Enter Join date.")]
        [Display(Name = "Joining Date*")]
        public DateTime? JoinDate { get; set; }

        [Display(Name = "Relieved Date")]
        public DateTime? Relieved { get; set; }

        [Display(Name = "Is Active")]
        public bool Isactive { get; set; }
        public string CreatedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ModifiedOn { get; set; }

    }
}