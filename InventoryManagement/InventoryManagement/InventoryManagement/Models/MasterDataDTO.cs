using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class MasterDataDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter name.")]
        public string MasterName { get; set; }
        public string Description { get; set; }
        //public string Type { get; set; }
        public MasterDataType Type { get; set; }
        public bool Isactive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }


        public static string GetDisplayName(Enum enumValue)
        {
            return enumValue.GetType()?
                        .GetMember(enumValue.ToString())?
                        .First()?
                        .CustomAttributes.FirstOrDefault()?.NamedArguments?.FirstOrDefault().TypedValue.Value.ToString();
        }
    }

    public enum MasterDataType
    {
        None = 0,
        [Display(Name = "Brand")]
        Brand = 1,
        [Display(Name = "Variety")]
        Variety = 2,
        [Display(Name = "Specie")]
        Specie = 3,
        [Display(Name = "Product Form")]
        ProductForm = 4,
        [Display(Name = "Freezing Type")]
        FreezingType = 5,
        [Display(Name = "Packing Type")]
        PackingType = 6,
        [Display(Name = "Grades")]
        Grades = 7
    }
}