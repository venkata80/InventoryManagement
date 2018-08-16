using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class SuppliersDTO : BaseInformation
    {
        public string SupplierBussinessName { get; set; }
        public string PlantName { get; set; }
        public string CoretYpeName { get; set; }
        public string ExpertedDays { get; set; }
        public string SACCode { get; set; }
        public string GSTNumber { get; set; }
        public int Plantid { get; set; }
        public int  CoreTypeId { get; set; }

    }
}