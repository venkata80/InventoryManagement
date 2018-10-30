using InventoryManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class ProductTypeTaxesDTO : CommonBaseDTO<Guid>
    {
        public long? ProductTaxeId { get; set; }
        public long? ProductTypeId { get; set; }
        public long? SGST { get; set; }
        public long? CGST { get; set; }
        public long? IGST { get; set; }
        public DateTime? AffectiveFrom { get; set; }
        public DateTime? AffectiveTo { get; set; }
        public bool Isactive { get; set; }
        public string ProductTypeName
        {
            get
            {
                string name = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(ProductTypeId) > int.MinValue)
                    {
                        name = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.ProductType && c.Id == ProductTypeId)?.MasterName;
                    }
                }
                return name;
            }
        }


    }
}