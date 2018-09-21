//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InventoryManagement.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        public System.Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<long> Type { get; set; }
        public string ShortCode { get; set; }
        public Nullable<long> Brand { get; set; }
        public Nullable<long> ProductForm { get; set; }
        public Nullable<long> Variety { get; set; }
        public Nullable<long> Specie { get; set; }
        public Nullable<long> FreezingType { get; set; }
        public Nullable<long> PackingType { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<long> PackingStyle { get; set; }
        public Nullable<long> Grade { get; set; }
        public Nullable<int> Ply { get; set; }
        public Nullable<int> PrintType { get; set; }
        public Nullable<int> TopType { get; set; }
        public string Dimensions { get; set; }
        public Nullable<int> ThresholdLimit { get; set; }
        public Nullable<bool> Isactive { get; set; }
        public System.Guid CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public System.Guid ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> Soaked { get; set; }
    
        public virtual aspnet_Users aspnet_Users { get; set; }
        public virtual aspnet_Users aspnet_Users1 { get; set; }
        public virtual MasterData MasterData { get; set; }
        public virtual MasterData MasterData1 { get; set; }
        public virtual MasterData MasterData2 { get; set; }
        public virtual MasterData MasterData3 { get; set; }
        public virtual MasterData MasterData4 { get; set; }
        public virtual MasterData MasterData5 { get; set; }
        public virtual MasterData MasterData6 { get; set; }
        public virtual MasterData MasterData7 { get; set; }
        public virtual MasterData MasterData8 { get; set; }
    }
}
