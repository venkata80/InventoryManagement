using InventoryManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace InventoryManagement.Models
{
    public class ProductDTO : CommonBaseDTO<Guid>
    {
        public long? Brand { get; set; }
        public string BrandName {
            get
            {
                string bname = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(Brand) > int.MinValue)
                    {
                        bname=MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Brand && c.Id == Brand)?.MasterName;
                    }
                }
                return bname;
            }  }

        public long? ProductForm { get; set; }
        public long? Variety { get; set; }
        public long? Specie { get; set; }
        public long? FreezingType { get; set; }
        public long? PackingType { get; set; }
        public long? Grade { get; set; }
        public int? Ply { get; set; }
        public long? Category { get; set; }
        public long? Type { get; set; }
        public long? Quantity { get; set; }
        public long? PackingStyle { get; set; }
        public long? Unit { get; set; }
        public string Name
        {
            get
            {
                StringBuilder name = new StringBuilder();
                if (HttpContext.Current !=null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (!string.IsNullOrWhiteSpace(ShortCode))
                    {
                        name.Append(ShortCode);
                        name.Append("-");
                    }
                    if (Convert.ToInt32(Brand) > int.MinValue)
                    {
                        name.Append(MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Brand && c.Id == Brand)?.MasterName);
                        name.Append("-");
                    }
                    if (Convert.ToInt32(ProductForm) > int.MinValue)
                    {
                        name.Append(MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.ProductForm && c.Id == ProductForm)?.MasterName);
                        name.Append("-");
                    }
                    if (Convert.ToInt32(Variety) > int.MinValue)
                    {
                        name.Append(MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Variety && c.Id == Variety)?.MasterName);
                        name.Append("-");
                    }
                    if (Convert.ToInt32(Specie) > int.MinValue)
                    {
                        name.Append(MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Specie && c.Id == Specie)?.MasterName);
                        name.Append("-");
                    }
                    if (Convert.ToInt32(FreezingType) > int.MinValue)
                    {
                        name.Append(MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.FreezingType && c.Id == FreezingType)?.MasterName);
                        name.Append("-");
                    }
                    if (Convert.ToInt32(PackingType) > int.MinValue)
                    {
                        name.Append(MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.PackingType && c.Id == PackingType)?.MasterName);
                        name.Append("-");
                    }
                    if (Convert.ToInt32(Soaked) > int.MinValue)
                    {
                        name.Append(MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.PackingType && c.Id == Soaked)?.MasterName);
                        name.Append("-");
                    }
                    if (Convert.ToInt32(Grade) > int.MinValue)
                    {
                        name.Append(MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Grades && c.Id == Grade)?.MasterName);
                        name.Append("-");
                    }
                    if (Convert.ToInt32(Quantity) > int.MinValue && Convert.ToInt32(PackingStyle) > int.MinValue)
                    {
                        name.Append(Quantity);
                        name.Append("x");
                        name.Append(MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.PackingType && c.Id == PackingStyle)?.MasterName);
                    }
                }
                return name.ToString();
            }
        }
        public string ShortCode { get; set; }
        public string Dimensions { get; set; }
        public long? NetWeight { get; set; }
        public string ThresholdLimit { get; set; }
        public string Description { get; set; }
        public long? CoreType { get; set; }
        public long? Soaked { get; set; }
        public long? Print { get; set; }
        public long? Top { get; set; }
        public bool Isactive { get; set; }
        public FileUploadDTO UploadImage { get; set; } = new FileUploadDTO();

        public string FreezingTypeName
        {
            get
            {
                string Fname = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MasterData"] != null)
                {
                    List<MasterDataDTO> MasterDataDetails = (List<MasterDataDTO>)HttpContext.Current.Session["MasterData"];
                    if (Convert.ToInt32(Brand) > int.MinValue)
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
                    if (Convert.ToInt32(Brand) > int.MinValue)
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
                    if (Convert.ToInt32(Brand) > int.MinValue)
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
                    if (Convert.ToInt32(Brand) > int.MinValue)
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
                    if (Convert.ToInt32(Brand) > int.MinValue)
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
                    if (Convert.ToInt32(Brand) > int.MinValue)
                    {
                        name = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Grades && c.Id == Grade)?.MasterName;
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
                    if (Convert.ToInt32(Brand) > int.MinValue)
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
                    if (Convert.ToInt32(Brand) > int.MinValue)
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
                    if (Convert.ToInt32(Brand) > int.MinValue)
                    {
                        name = MasterDataDetails.FirstOrDefault(c => c.Type == MasterDataType.Soaked && c.Id == Soaked)?.MasterName;
                    }
                }
                return name;
            }
        }


    }
}