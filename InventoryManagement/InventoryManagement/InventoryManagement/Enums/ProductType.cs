using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Enums
{
    public enum ProductCoreType
    {
        None = 0,
        [Display(Name = "Core Item")]
        Core_Item = 1,
        [Display(Name = "Non Core Item")]
        Non_Core_Item = 2
    }
}