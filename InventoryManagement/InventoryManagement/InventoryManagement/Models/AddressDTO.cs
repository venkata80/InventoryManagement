using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class AddressDTO : CommonBaseDTO<Guid>
    {
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
    }
}