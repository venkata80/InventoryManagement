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
        public Guid ProductId { get; set; }

        public List<SuppliersDTO> SupplierList { get; set; }
        public List<ProductDTO> ProductList { get; set; }

        public long? Brand { get; set; }
        public long? FreezingType { get; set; }
        public long? Variety { get; set; }
        public long? Specie { get; set; }
        public long? PackingType { get; set; }
        public long? ProductForm { get; set; }
        public long? Type { get; set; }
        public long? Grade { get; set; }
        public long? Ply { get; set; }
        public long? Category { get; set; }
        public long? CoreType { get; set; }
        public long? Soaked { get; set; }
        public long? PackingCount { get; set; }
        public long? VenderUnits { get; set; }
        public string ExpectedDays { get; set; }
        public bool IsActive { get; set; }
    }
}