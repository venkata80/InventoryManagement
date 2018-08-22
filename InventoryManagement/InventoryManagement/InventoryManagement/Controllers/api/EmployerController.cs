using InventoryManagement.EF;
using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventoryManagement.Controllers.api
{
    public class EmployerController : ApiController
    {

        #region Employer

        // GET api/<controller>
        [HttpGet]
        [ActionName("GetEmployee")]
        public IHttpActionResult GetEmployer()
        {
            IList<BaseEmployerDTO> students = new List<BaseEmployerDTO>();

            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                students = hhh.IM_Employer.Select(s => new BaseEmployerDTO()
                {
                    Id = s.EMPID,
                    LastName = s.LastName,
                    FirstName = s.FirstName,
                    MiddleName = s.Middlename,
                    //Fullname=s.FirstName+ ' '+s.Middlename+' '+s.LastName,
                    Designation = s.Designation,
                    Dateofbirth = s.DOB,
                    gender = s.Gender,
                    Email=s.Email,
                    ResPhone = s.ResPhone,
                    CellPhone = s.CellPhone,
                    Address = s.Address,
                    City = s.City,
                    State = s.State,
                    Zipcode = s.Zipcode,
                    JoinDate = s.JoinDate,
                    Relieved = s.RelievedDate,
                    Isactive = true
                }).ToList<BaseEmployerDTO>();
                if (students == null)
                    students = new List<BaseEmployerDTO>();
            }
            if (students.Count == 0)
                return NotFound();
            return Ok(students);
        }

        [HttpPost]
        [ActionName("InsertEmployerData")]
        public IHttpActionResult PostNewStudent([FromBody]BaseEmployerDTO s)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            using (var ctx = new InventoryManagementEntities())
            {
                ctx.IM_Employer.Add(new IM_Employer()
                {
                    EMPID = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Middlename = s.MiddleName,
                    Designation = s.Designation,
                    DOB = s.Dateofbirth,
                    Gender = s.gender,
                    ResPhone = s.ResPhone,
                    CellPhone = s.CellPhone,
                    Address = s.Address,
                    City = s.City,
                    State = s.State,
                    Zipcode = s.Zipcode,
                    JoinDate = s.JoinDate,
                    RelievedDate = s.Relieved,
                    IsActive = s.Isactive,
                    CreateBy = s.CreatedBy,
                    CreatedDate = DateTime.Now,

                });

                ctx.SaveChanges();
            }

            return Ok();
        }


        [HttpPut]
        [ActionName("UpdateEmployerData")]
        public IHttpActionResult PutStudent(BaseEmployerDTO s)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            using (var ctx = new InventoryManagementEntities())
            {

                var existingSupplier = ctx.IM_Employer.Where(s1 => s1.EMPID == s.Id)
                                                   .FirstOrDefault<IM_Employer>();
                if (existingSupplier != null)
                {

                    existingSupplier.EMPID = s.Id;
                    existingSupplier.FirstName = s.FirstName;
                    existingSupplier.LastName = s.LastName;
                    existingSupplier.Middlename = s.MiddleName;
                    existingSupplier.Designation = s.Designation;
                    existingSupplier.DOB = s.Dateofbirth;
                    existingSupplier.Gender = s.gender;
                    existingSupplier.ResPhone = s.ResPhone;
                    existingSupplier.CellPhone = s.CellPhone;
                    existingSupplier.Address = s.Address;
                    existingSupplier.City = s.City;
                    existingSupplier.State = s.State;
                    existingSupplier.Zipcode = s.Zipcode;
                    existingSupplier.JoinDate = s.JoinDate;
                    existingSupplier.RelievedDate = s.Relieved;
                    existingSupplier.IsActive = s.Isactive;
                    existingSupplier.ModifiedBy = s.ModifiedBy;
                    existingSupplier.ModifiedDate = DateTime.Now;
                    ctx.SaveChanges();
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok();
        }

        [HttpDelete]
        [ActionName("DeleteEmployerData")]
        public IHttpActionResult DeleteStudent(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            using (var ctx = new InventoryManagementEntities())
            {

                var student = ctx.IM_Employer
               .Where(s => s.EMPID == id)
               .FirstOrDefault();

                ctx.Entry(student).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }

            return Ok();
        }
        // GET api/<controller>/5
        [HttpGet]
        [ActionName("GetEmployeeById")]
        public IHttpActionResult GetEmployerBy(int id)
        {
            BaseEmployerDTO Supplier = new BaseEmployerDTO();
            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                IM_Employer s = hhh.IM_Employer
              .Where(s1 => s1.EMPID == id)
              .FirstOrDefault();
                if (s != null)
                {
                    Supplier.Id = s.EMPID;
                    Supplier.FirstName = s.FirstName;
                    Supplier.LastName = s.LastName;
                    Supplier.MiddleName = s.Middlename;
                    Supplier.Designation = s.Designation;
                    Supplier.Dateofbirth = s.DOB;
                    Supplier.gender = s.Gender;
                    Supplier.ResPhone = s.ResPhone;
                    Supplier.CellPhone = s.CellPhone;
                    Supplier.Address = s.Address;
                    Supplier.City = s.City;
                    Supplier.State = s.State;
                    Supplier.Zipcode = s.Zipcode;
                    Supplier.JoinDate = s.JoinDate;
                    Supplier.Relieved = s.RelievedDate;
                    Supplier.Isactive = s.IsActive.Value;
                    Supplier.ModifiedBy = s.ModifiedBy;
                    Supplier.ModifiedOn = DateTime.Now;

                }
            }
            if (Supplier.Id == 0)
                return NotFound();
            return Ok(Supplier);
        }


        #endregion

        #region Suppliers
        [HttpGet]
        [ActionName("GetAllSuppliers")]
        public IHttpActionResult GetEUppliers()
        {
            IList<SuppliersDTO> SUPPLIER = null;

            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                SUPPLIER = hhh.IM_SUPPLIER.Select(s => new SuppliersDTO()
                {
                    Id = s.ID,
                    CoreTypeId = s.CoreTypeId.Value,
                    Plantid = s.PlantId.Value,
                    SupplierBussinessName = s.BusinessName,
                    Fullname = s.Name,
                    Address = s.Address,
                    City = s.City,
                    State = s.State,
                    Zipcode = s.Zipcode,
                    ExpertedDays = s.ExpectedDays,
                    Email = s.EmailId,
                    CellPhone = s.Phone,
                    SACCode = s.SACcode,
                    GSTNumber = s.GSTno,
                    Isactive = s.IsActive.Value,
                    CreatedBy = s.CreatedBy,
                    ModifiedBy = s.ModifiedBy,
                    CreatedOn = s.CreatedOn,
                    ModifiedOn = s.ModifiedOn
                }).ToList<SuppliersDTO>();
            }
            if (SUPPLIER.Count == 0)
                return NotFound();
            return Ok(SUPPLIER);
        }
        [HttpPost]
        [ActionName("InsertSupplierData")]
        public IHttpActionResult PostNewSuppliers([FromBody]SuppliersDTO s)
        {
            using (var ctx = new InventoryManagementEntities())
            {
                ctx.IM_SUPPLIER.Add(new IM_SUPPLIER()
                {
                    ID = s.Id,
                    CoreTypeId = s.CoreTypeId,
                    PlantId = s.Plantid,
                    BusinessName = s.SupplierBussinessName,
                    Name = s.Fullname,
                    Address = s.Address,
                    City = s.City,
                    State = s.State,
                    Zipcode = s.Zipcode,
                    ExpectedDays = s.ExpertedDays,
                    EmailId = s.Email,
                    Phone = s.CellPhone,
                    SACcode = s.SACCode,
                    GSTno = s.GSTNumber,
                    IsActive = s.Isactive,
                    CreatedBy = s.CreatedBy,
                    CreatedOn = DateTime.Now,

                });

                ctx.SaveChanges();
            }

            return Ok();
        }
        [HttpPut]
        [ActionName("UpdateSupplierData")]
        public IHttpActionResult PutSupplier(SuppliersDTO s)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            using (var ctx = new InventoryManagementEntities())
            {

                var existingSupplier = ctx.IM_SUPPLIER.Where(s1 => s1.ID == s.Id)
                                                   .FirstOrDefault<IM_SUPPLIER>();
                if (existingSupplier != null)
                {
                    existingSupplier.ID = s.Id;
                    existingSupplier.CoreTypeId = s.CoreTypeId;
                    existingSupplier.PlantId = s.Plantid;
                    existingSupplier.BusinessName = s.SupplierBussinessName;
                    existingSupplier.Name = s.Fullname;
                    existingSupplier.Address = s.Address;
                    existingSupplier.City = s.City;
                    existingSupplier.State = s.State;
                    existingSupplier.Zipcode = s.Zipcode;
                    existingSupplier.ExpectedDays = s.ExpertedDays;
                    existingSupplier.EmailId = s.Email;
                    existingSupplier.Phone = s.CellPhone;
                    existingSupplier.SACcode = s.SACCode;
                    existingSupplier.GSTno = s.GSTNumber;
                    existingSupplier.IsActive = s.Isactive;
                    existingSupplier.ModifiedBy = s.ModifiedBy;
                    existingSupplier.ModifiedOn = DateTime.Now;
                    ctx.SaveChanges();
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok();
        }
        [HttpDelete]
        [ActionName("DeleteSupplierData")]
        public IHttpActionResult DeleteSupplier(int id)
        {
            using (var ctx = new InventoryManagementEntities())
            {

                var student = ctx.IM_SUPPLIER
               .Where(s => s.ID == id)
               .FirstOrDefault();

                ctx.Entry(student).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }

            return Ok();
        }
        [HttpGet]
        [ActionName("GetSupplierById")]
        public IHttpActionResult GetSupplierBy(int id)
        {
            SuppliersDTO Supplier = new SuppliersDTO();
            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                IM_SUPPLIER s = hhh.IM_SUPPLIER.Where(s1 => s1.ID == id).FirstOrDefault();
                if (s != null)
                {
                    Supplier.Id = s.ID;
                    Supplier.CoreTypeId = s.CoreTypeId.Value;
                    Supplier.Plantid = s.PlantId.Value;
                    Supplier.SupplierBussinessName = s.BusinessName;
                    Supplier.Fullname = s.Name;
                    Supplier.Address = s.Address;
                    Supplier.City = s.City;
                    Supplier.State = s.State;
                    Supplier.Zipcode = s.Zipcode;
                    Supplier.ExpertedDays = s.ExpectedDays;
                    Supplier.Email = s.EmailId;
                    Supplier.CellPhone = s.Phone;
                    Supplier.SACCode = s.SACcode;
                    Supplier.GSTNumber = s.GSTno;
                    Supplier.Isactive = s.IsActive.Value;
                    Supplier.ModifiedBy = s.ModifiedBy;
                    Supplier.ModifiedOn = DateTime.Now;
                }
            }
            if (Supplier.Id == 0)
                return NotFound();
            return Ok(Supplier);
        }
        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        #endregion

        [HttpGet]
        [ActionName("GetMasterDataBy")]
        public IHttpActionResult GetMasterData()
        {
            List<MasterDataDTO> MasterBytypelist = MaterDataList();
            if (MasterBytypelist.Count == 0)
                return NotFound();
            return Ok(MasterBytypelist);

        }

        [HttpGet]
        [ActionName("GetMasterDataByID")]
        public IHttpActionResult GetMasterData(long id)
        {
            List<MasterDataDTO> MasterBytypelist = MaterDataList(id);
            if (MasterBytypelist.Count == 0)
                return NotFound();
            return Ok(MasterBytypelist);

        }

        [HttpPost]
        [ActionName("InsertMasterdata")]
        public IHttpActionResult PostNewMasterdate([FromBody]MasterDataDTO s)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            using (var ctx = new InventoryManagementEntities())
            {
                if (s.Id != 0)
                {
                    var Masterdata = ctx.IM_MasterData.Where(s1 => s1.Id == s.Id)
                                                      .FirstOrDefault<IM_MasterData>();
                    if (Masterdata != null)
                    {
                        Masterdata.Id = s.Id;
                        Masterdata.MasterName = s.MasterName;
                        Masterdata.Descrption = s.Description;
                        Masterdata.Isactive = s.Isactive;
                        Masterdata.ModifiedBy = s.ModifiedBy;
                        Masterdata.ModifiedOn = DateTime.Now;
                        //ctx.SaveChanges();
                    }


                }
                else
                {
                    ctx.IM_MasterData.Add(new IM_MasterData()
                    {
                        Id = s.Id,
                        MasterName = s.MasterName,
                        Descrption = s.Description,
                        Type = s.Type.ToString(),
                        Isactive = s.Isactive,
                        CreatedBy = s.CreatedBy,
                        CreatedOn = DateTime.Now,

                    });
                }
                ctx.SaveChanges();
            }

            return Ok();
        }

        [ActionName("DeleteMasterdata")]
        // DELETE api/<controller>/5
        public IHttpActionResult DeleteMasterdata(int id)
        {
            using (var ctx = new InventoryManagementEntities())
            {

                var masterdata = ctx.IM_MasterData
               .Where(s => s.Id == id)
               .FirstOrDefault();

                ctx.Entry(masterdata).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }

            return Ok();
        }

        List<MasterDataDTO> MaterDataList(long id = 0)
        {
            List<MasterDataDTO> MasterBytypelist = new List<MasterDataDTO>();
            List<IM_MasterData> imMaster = new List<IM_MasterData>();
            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                imMaster = hhh.IM_MasterData.ToList();
                if (imMaster != null && imMaster.Any())
                {
                    if (id == 0)
                    {
                        MasterBytypelist = imMaster.Select(item => new MasterDataDTO()
                        {
                            Id = item.Id,
                            MasterName = item.MasterName,
                            Description = item.Descrption,
                            Type = (MasterDataType)Enum.Parse(typeof(MasterDataType), item.Type),
                            Isactive = item.Isactive.Value,
                            ModifiedBy = item.ModifiedBy,
                            ModifiedOn = item.ModifiedOn,
                            CreatedBy = item.CreatedBy,
                            CreatedOn = item.CreatedOn
                        }).ToList();
                    }
                    else
                    {
                        MasterBytypelist = imMaster.Where(c => c.Id == id)?.Select(item => new MasterDataDTO()
                        {
                            Id = item.Id,
                            MasterName = item.MasterName,
                            Description = item.Descrption,
                            Type = (MasterDataType)Enum.Parse(typeof(MasterDataType), item.Type),
                            Isactive = item.Isactive.Value,
                            ModifiedBy = item.ModifiedBy,
                            ModifiedOn = item.ModifiedOn,
                            CreatedBy = item.CreatedBy,
                            CreatedOn = item.CreatedOn
                        }).ToList();
                    }
                }
            }
            return MasterBytypelist;
        }
    }
}