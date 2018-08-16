using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string Fullname { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Dateofbirth { get; set; }
        public string gender { get; set; }
        [DataType(DataType.PhoneNumber)]
        public int? ResPhone { get; set; }
        [DataType(DataType.PhoneNumber)]
        public int? CellPhone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [DataType(DataType.PostalCode)]
        public int? Zipcode { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? Relieved { get; set; }
        public bool Isactive { get; set; }
        public string CreatedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ModifiedOn { get; set; }

    }
}