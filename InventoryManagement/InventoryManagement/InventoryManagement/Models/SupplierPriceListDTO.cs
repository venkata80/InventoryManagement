using InventoryManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class SupplierPriceListDTO : CommonBaseDTO<Guid>
    {
        public Guid SupplierId { get; set; }
        public List<SuppliersDTO> SupplierList { get; set; }
        public int Brand { get; set; }
        public int FreezingType { get; set; }
        public int Variety { get; set; }
        public int Specie { get; set; }
        public int PackingType { get; set; }
        public int ProductForm { get; set; }
        public int Type { get; set; }
        public int Grade { get; set; }
        public int Ply { get; set; }
        public int Category { get; set; }
        public ProductCoreType CoreType { get; set; }
        public ProductSoakedType Soaked { get; set; }
        public string PackingCount { get; set; }
        public long VenderUnits { get; set; }
        public string ExpectedDays { get; set; }
    }
}