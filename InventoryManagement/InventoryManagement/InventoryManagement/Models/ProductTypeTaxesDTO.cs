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
    }
}