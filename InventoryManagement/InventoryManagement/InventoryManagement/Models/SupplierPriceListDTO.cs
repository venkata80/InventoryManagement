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
        public long? Type { get; set; }

        public List<SuppliersDTO> SupplierList { get; set; }
        public List<ProductDTO> ProductList { get; set; }

        public long? Brand { get; set; }
        public long? FreezingType { get; set; }
        public long? Variety { get; set; }
        public long? Specie { get; set; }
        public long? PackingType { get; set; }
        public long? ProductForm { get; set; }
        public long? Grade { get; set; }
        public long? ProductType { get; set; }
        public long? Ply { get; set; }
        public long? Category { get; set; }
        public long? CoreType { get; set; }
        public long? Soaked { get; set; }
        public long? PackingCount { get; set; }
        public long? VenderUnits { get; set; }
        public string ExpectedDays { get; set; }
        public bool IsActive { get; set; }
        //Display
      //  public Guid SupplierId { get; set; }
        public string SupplierName
        {
            get
            {
                string bname = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["SupplierList"] != null)
                {
                    List<SuppliersDTO> MasterDataDetails = (List<SuppliersDTO>)HttpContext.Current.Session["SupplierList"];
                    if (SupplierId !=Guid.Empty)
                    {
                        bname = MasterDataDetails.FirstOrDefault(c => c.Id == SupplierId)?.SupplierBussinessName;
                    }
                }
                return bname;
            }
        }

        public string BrandName
        {
            get
            {
                string bname = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(Brand) > int.MinValue)
                    {
                        bname = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Brand && c.Id == Brand)?.MasterName;
                    }
                }
                return bname;
            }
        }
        public string FreezingTypeName
        {
            get
            {
                string Fname = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(FreezingType) > int.MinValue)
                    {
                        Fname = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.FreezingType && c.Id == FreezingType)?.MasterName;
                    }
                }
                return Fname;
            }
        }
        public string VarietyName
        {
            get
            {
                string Fname = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(Variety) > int.MinValue)
                    {
                        Fname = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Variety && c.Id == Variety)?.MasterName;
                    }
                }
                return Fname;
            }
        }
        public string SpecieName
        {
            get
            {
                string Fname = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(Specie) > int.MinValue)
                    {
                        Fname = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Specie && c.Id == Specie)?.MasterName;
                    }
                }
                return Fname;
            }
        }
        public string PackingTypeName
        {
            get
            {
                string Fname = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(PackingType) > int.MinValue)
                    {
                        Fname = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.PackingType && c.Id == PackingType)?.MasterName;
                    }
                }
                return Fname;
            }
        }
        public string ProductFormName
        {
            get
            {
                string name = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(ProductForm) > int.MinValue)
                    {
                        name = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.ProductForm && c.Id == ProductForm)?.MasterName;
                    }
                }
                return name;
            }
        }
        public string GradeName
        {
            get
            {
                string name = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(Grade) > int.MinValue)
                    {
                        name = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Grades && c.Id == Grade)?.MasterName;
                    }
                }
                return name;
            }
        }
        public string ProductTypeName
        {
            get
            {
                string name = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(ProductType) > int.MinValue)
                    {
                        name = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.ProductType && c.Id == ProductType)?.MasterName;
                    }
                }
                return name;
            }
        }
        public string PlyName
        {
            get
            {
                string name = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(Ply) > int.MinValue)
                    {
                        name = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Ply && c.Id == Ply)?.MasterName;
                    }
                }
                return name;
            }
        }
        public string CategoryName
        {
            get
            {
                string name = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(Category) > int.MinValue)
                    {
                        name = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.PoductCategory && c.Id == Category)?.MasterName;
                    }
                }
                return name;
            }
        }
        public string SoakedName
        {
            get
            {
                string name = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(Soaked) > int.MinValue)
                    {
                        name = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Soaked && c.Id == Soaked)?.MasterName;
                    }
                }
                return name;
            }
        }
    }
}