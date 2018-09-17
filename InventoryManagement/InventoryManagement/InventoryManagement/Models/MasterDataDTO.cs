using InventoryManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class MasterDataDTO : CommonBaseDTO<long>
    {
        [Required(ErrorMessage = "Please enter name.")]
        public string MasterName { get; set; }
        public string Description { get; set; }
        public string TypeUnits { get; set; }
        public MasterDataType Type { get; set; }
        public bool Isactive { get; set; }


        public static string GetDisplayName(Enum enumValue)
        {
            return enumValue.GetType()?
                        .GetMember(enumValue.ToString())?
                        .First()?
                        .CustomAttributes.FirstOrDefault()?.NamedArguments?.FirstOrDefault().TypedValue.Value.ToString();
        }
    }


}