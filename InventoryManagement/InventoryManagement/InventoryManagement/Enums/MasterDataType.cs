using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Enums
{
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
        Grades = 7,
        [Display(Name = "Product Type")]
        ProductType = 8,
        [Display(Name = "Product Category")]
        PoductCategory = 9
    }
}