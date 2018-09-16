using InventoryManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class SuppliersDTO : UserDTO
    {
        public SuppliersDTO()
        {
            base.Role = RoleType.Supplier;
        }

        [Display(Name = "Supplier Bussiness Name*")]
        [Required(ErrorMessage = "Enter supplier bussiness name")]
        public string SupplierBussinessName { get; set; }

        public string PlantName { get; set; }
        
        public string CoretypeName { get; set; }

        [Display(Name = "Experted Days*")]
        [Required(ErrorMessage = "Enter experted days")]
        public string ExpertedDays { get; set; }

        [Display(Name = "SAC Code*")]
        [Required(ErrorMessage = "Enter SAC code")]
        public string SACCode { get; set; }

        [Display(Name = "GST Number*")]
        [Required(ErrorMessage = "Enter GST number")]
        public string GSTNumber { get; set; }

        [Display(Name = "Plant Name*")]
        [Required(ErrorMessage = "Enter plant name")]
        public int Plantid { get; set; }

        [Display(Name = "Plant CoretypeName*")]
        [Required(ErrorMessage = "Enter coretype name")]
        public int CoreTypeId { get; set; }

        public AddressDTO Address { get; set; }

    }
}