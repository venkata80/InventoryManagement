using InventoryManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class SuppliersDTO : UserDTO
    {
        [Display(Name = "Supplier Bussiness Name*")]
        [Required(ErrorMessage = "Enter supplier bussiness name")]
        public string SupplierBussinessName { get; set; }

        [Display(Name = "Plant Name*")]
        [Required(ErrorMessage = "Enter plant name")]
        public string PlantName { get; set; }

        [Display(Name = "Plant CoretYpeName*")]
        [Required(ErrorMessage = "Enter coretYpe name")]
        public string CoretYpeName { get; set; }

        [Display(Name = "Experted Days*")]
        [Required(ErrorMessage = "Enter experted days")]
        public string ExpertedDays { get; set; }

        [Display(Name = "SAC Code*")]
        [Required(ErrorMessage = "Enter SAC code")]
        public string SACCode { get; set; }

        [Display(Name = "GST Number*")]
        [Required(ErrorMessage = "Enter GST number")]
        public string GSTNumber { get; set; }

        public int Plantid { get; set; }

        public int CoreTypeId { get; set; }

        public AddressDTO Address { get; set; }

    }
}