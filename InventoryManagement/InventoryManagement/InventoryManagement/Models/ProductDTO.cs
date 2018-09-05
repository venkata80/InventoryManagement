using InventoryManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class ProductDTO : CommonBaseDTO<Guid>
    {
        public int Brand { get; set; }
        public int ProductForm { get; set; }
        public int Variety { get; set; }
        public int Specie { get; set; }
        public int FreezingType { get; set; }
        public int PackingType { get; set; }
        public int Grade { get; set; }
        public int Ply { get; set; }
        public int Category { get; set; }
        public int Type { get; set; }
        public string ShortCode { get; set; }
        public string Dimensions { get; set; }
        public string NetWeight { get; set; }
        public string ThresholdLimit { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> PackingStyle { get; set; }
        public ProductCoreType CoreType { get; set; }
        public ProductSoakedType Soaked { get; set; }
        public ProductPrint Print { get; set; }
        public ProductTop Top { get; set; }
        public FileUploadDTO UploadImage { get; set; }

    }
}