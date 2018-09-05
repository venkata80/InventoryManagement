using InventoryManagement.EF;
using InventoryManagement.Enums;
using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Security;

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
            IList<EmployerDTO> employers = null;

            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                employers = hhh.Employers.Select(s => new EmployerDTO()
                {
                    Id = s.ID,
                    LastName = s.User.LastName,
                    FirstName = s.User.FirstName,
                    MiddleName = s.User.MiddleName,
                    //Fullname=s.FirstName+ ' '+s.Middlename+' '+s.LastName,
                    Designation = s.Designation,
                    Dateofbirth = s.DOB,
                    gender = s.Gender,
                    Email = s.User.Email,
                    ResPhone = s.ResPhone,
                    CellPhone = s.CellPhone,
                    Address = new AddressDTO
                    {
                        Address = s.Address.Address1,
                        City = s.Address.City,
                        State = s.Address.State,
                        Zipcode = s.Address.Zipcode,
                    },
                    JoinDate = s.JoinDate,
                    Relieved = s.RelievedDate,
                    Isactive = true
                }).ToList<EmployerDTO>();
                if (employers == null)
                    employers = new List<EmployerDTO>();
            }
            if (employers == null || !employers.Any())
                return NotFound();
            return Ok(employers);
        }

        [HttpPost]
        //[ActionName("InsertEmployerData")]
        public IHttpActionResult SaveEmployer([FromBody]EmployerDTO s)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");
            Guid employerid = Guid.Empty;
            using (var ctx = new InventoryManagementEntities())
            {
                try
                {
                    if (s.Id == Guid.Empty)
                    {
                        UserDTO user = new UserController().CreateUser(new UserDTO { Id = s.Id, Email = s.Email, FirstName = s.FirstName, LastName = s.LastName, MiddleName = s.MiddleName, Role = s.Role, Isactive = true });

                        Guid addressid = SaveAddress(s, user.Id);

                        if (addressid != Guid.Empty)
                        {
                            employerid = SaveEmployerDetails(s, user.Id, user.Id, addressid);
                        }
                    }
                    else
                    {
                        var existinguser = ctx.Users.FirstOrDefault(c => c.ID == s.Id);
                        if(existinguser!=null)
                        {
                            existinguser.FirstName = s.FirstName;
                            existinguser.LastName = s.LastName;
                            existinguser.MiddleName = s.MiddleName;
                            existinguser.ModifiedBy = s.ModifiedBy;
                            existinguser.ModifiedDate = DateTime.UtcNow;

                            ctx.SaveChanges();
                        }

                        SaveAddress(s, s.ModifiedBy);
                        SaveEmployerDetails(s, s.Id, s.ModifiedBy, s.Address.Id);
                    }
                }
                catch (Exception ex)
                {
                    if (s.Id == Guid.Empty)
                    {
                        DeleteUserDetails(s.Email);
                    }
                }
            }

            if (employerid == Guid.Empty)
            {
                DeleteUserDetails(s.Email);
            }
            return Ok();
        }

        Guid SaveAddress(EmployerDTO employerdto, Guid createdby)
        {
            Guid addressid = employerdto.Id == Guid.Empty ? Guid.NewGuid() : employerdto.Id;
            try
            {
                using (var addressctx = new InventoryManagementEntities())
                {
                    Address address = new Address();
                    address.ID = addressid;
                    address.Address1 = employerdto.Address.Address;
                    address.City = employerdto.Address.City;
                    address.State = employerdto.Address.State;
                    address.Zipcode = employerdto.Address.Zipcode;
                    if (employerdto.Id == Guid.Empty)
                    {
                        address.CreatedBy = createdby;
                        address.CreatedDate = DateTime.UtcNow;
                    }
                    address.ModifiedBy = createdby;
                    if (employerdto.Id != Guid.Empty)
                        address.ModifiedDate = DateTime.UtcNow;

                    address.ActiveFL = employerdto.Isactive;

                    addressctx.Addresses.Add(address);
                    addressctx.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                return Guid.Empty;
            }
            return addressid;
        }

        Guid SaveEmployerDetails(EmployerDTO employerdto, Guid userid, Guid createdby, Guid addressid)
        {
            try
            {
                using (var employerctx = new InventoryManagementEntities())
                {
                    Employer emp = new Employer();
                    emp.ID = userid;
                    emp.Designation = employerdto.Designation;
                    emp.DOB = employerdto.Dateofbirth;
                    emp.Gender = employerdto.gender;
                    emp.ResPhone = employerdto.ResPhone;
                    emp.CellPhone = employerdto.CellPhone;
                    emp.AddressID = addressid;
                    emp.JoinDate = employerdto.JoinDate.HasValue ? employerdto.JoinDate.Value.Date : DateTime.MinValue;
                    emp.RelievedDate = employerdto.Relieved.HasValue ? employerdto.Relieved.Value.Date : DateTime.MinValue;
                    if (employerdto.Id == Guid.Empty)
                    {
                        emp.CreatedBy = createdby;
                        emp.CreatedDate = DateTime.UtcNow;
                    }
                    emp.ModifiedBy = createdby;

                    if (employerdto.Id != Guid.Empty)
                        emp.ModifiedDate = DateTime.UtcNow;

                    employerctx.Employers.Add(emp);
                    employerctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
            return userid;
        }

        void DeleteUserDetails(string email)
        {
            try
            {
                using (var deleteuserctx = new InventoryManagementEntities())
                {
                    var member = deleteuserctx.aspnet_Membership.FirstOrDefault(c => c.Email == email);
                    if (member != null)
                    {
                        Membership.DeleteUser(email);
                        var user = deleteuserctx.Users.FirstOrDefault(c => c.ID == member.UserId);
                        if(user!=null)
                        {
                            var employer = deleteuserctx.Employers.FirstOrDefault(c => c.ID == member.UserId);
                            if (employer != null)
                            {
                                var address = deleteuserctx.Addresses.FirstOrDefault(c => c.ID == employer.AddressID);
                                if (address != null)
                                {
                                    deleteuserctx.Entry(address).State = System.Data.Entity.EntityState.Deleted;
                                    deleteuserctx.SaveChanges();
                                }

                                deleteuserctx.Entry(employer).State = System.Data.Entity.EntityState.Deleted;
                                deleteuserctx.SaveChanges();
                            }
                            deleteuserctx.Entry(user).State = System.Data.Entity.EntityState.Deleted;
                            deleteuserctx.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        //[HttpPut]
        //[ActionName("UpdateEmployerData")]
        //public IHttpActionResult PutStudent(EmployerDTO s)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest("Not a valid model");

        //    using (var ctx = new InventoryManagementEntities())
        //    {

        //        //var existingSupplier = ctx.IM_Employer.Where(s1 => s1.EMPID == s.Id)
        //        //                                   .FirstOrDefault<IM_Employer>();
        //        //if (existingSupplier != null)
        //        //{

        //        //    existingSupplier.EMPID = s.Id;
        //        //    existingSupplier.FirstName = s.FirstName;
        //        //    existingSupplier.LastName = s.LastName;
        //        //    existingSupplier.Middlename = s.MiddleName;
        //        //    existingSupplier.Designation = s.Designation;
        //        //    existingSupplier.DOB = s.Dateofbirth;
        //        //    existingSupplier.Gender = s.gender;
        //        //    existingSupplier.ResPhone = s.ResPhone;
        //        //    existingSupplier.CellPhone = s.CellPhone;
        //        //    existingSupplier.Address = s.Address;
        //        //    existingSupplier.City = s.City;
        //        //    existingSupplier.State = s.State;
        //        //    existingSupplier.Zipcode = s.Zipcode;
        //        //    existingSupplier.JoinDate = s.JoinDate;
        //        //    existingSupplier.RelievedDate = s.Relieved;
        //        //    existingSupplier.IsActive = s.Isactive;
        //        //    existingSupplier.ModifiedBy = s.ModifiedBy;
        //        //    existingSupplier.ModifiedDate = DateTime.Now;
        //        //    ctx.SaveChanges();
        //        //}
        //        //else
        //        //{
        //        //    return NotFound();
        //        //}
        //    }

        //    return Ok();
        //}

        [HttpDelete]
        //[ActionName("DeleteEmployerData")]
        public IHttpActionResult DeleteEmployer(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            using (var ctx = new InventoryManagementEntities())
            {
                var user = ctx.Users.FirstOrDefault(c => c.ID == id);
                if(user!=null)
                {
                    user.ActiveFL = false;
                    ctx.SaveChanges();
                }
                
            }

            return Ok();
        }
        // GET api/<controller>/5
        [HttpGet]
        [ActionName("GetEmployeeById")]
        public IHttpActionResult GetEmployerBy(Guid id)
        {
            EmployerDTO employer = new EmployerDTO();
            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                DateTime? nullabledate = null;
                Employer s = hhh.Employers?.Where(s1 => s1.ID == id)?.FirstOrDefault();
                if (s != null)
                {
                    employer.Id = s.ID;
                    employer.FirstName = s.User.FirstName;
                    employer.LastName = s.User.LastName;
                    employer.MiddleName = s.User.MiddleName;
                    employer.Designation = s.Designation;
                    employer.Dateofbirth = s.DOB;
                    employer.gender = s.Gender;
                    employer.ResPhone = s.ResPhone;
                    employer.CellPhone = s.CellPhone;
                    employer.Email = s.User.Email;
                    employer.Address = new AddressDTO()
                    {
                        Id = s.Address.ID,
                        Address = s.Address.Address1,
                        City = s.Address.City,
                        State = s.Address.State,
                        Zipcode = s.Address.Zipcode
                    };
                    employer.JoinDate = s.JoinDate;
                    employer.Relieved = s.RelievedDate == DateTime.MinValue ? nullabledate : s.RelievedDate;
                    employer.Isactive = Convert.ToBoolean(s.User.ActiveFL);

                }
            }
            //if (Supplier.Id == 0)
            //    return NotFound();
            return Ok(employer);
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
                //SUPPLIER = hhh.IM_SUPPLIER.Select(s => new SuppliersDTO()
                //{
                //    Id = s.ID,
                //    CoreTypeId = s.CoreTypeId.Value,
                //    Plantid = s.PlantId.Value,
                //    SupplierBussinessName = s.BusinessName,
                //    //Fullname = s.Name,
                //    Address = s.Address,
                //    City = s.City,
                //    State = s.State,
                //    Zipcode = s.Zipcode,
                //    ExpertedDays = s.ExpectedDays,
                //    Email = s.EmailId,
                //    CellPhone = s.Phone,
                //    SACCode = s.SACcode,
                //    GSTNumber = s.GSTno,
                //    Isactive = s.IsActive.Value,
                //    CreatedBy = s.CreatedBy,
                //    ModifiedBy = s.ModifiedBy,
                //    CreatedOn = s.CreatedOn,
                //    ModifiedOn = s.ModifiedOn
                //}).ToList<SuppliersDTO>();
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
                //ctx.IM_SUPPLIER.Add(new IM_SUPPLIER()
                //{
                //    ID = s.Id,
                //    CoreTypeId = s.CoreTypeId,
                //    PlantId = s.Plantid,
                //    BusinessName = s.SupplierBussinessName,
                //    Name = s.Fullname,
                //    Address = s.Address,
                //    City = s.City,
                //    State = s.State,
                //    Zipcode = s.Zipcode,
                //    ExpectedDays = s.ExpertedDays,
                //    EmailId = s.Email,
                //    Phone = s.CellPhone,
                //    SACcode = s.SACCode,
                //    GSTno = s.GSTNumber,
                //    IsActive = s.Isactive,
                //    CreatedBy = s.CreatedBy,
                //    CreatedOn = DateTime.Now,

                //});

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

                //var existingSupplier = ctx.IM_SUPPLIER.Where(s1 => s1.ID == s.Id)
                //                                   .FirstOrDefault<IM_SUPPLIER>();
                //if (existingSupplier != null)
                //{
                //    existingSupplier.ID = s.Id;
                //    existingSupplier.CoreTypeId = s.CoreTypeId;
                //    existingSupplier.PlantId = s.Plantid;
                //    existingSupplier.BusinessName = s.SupplierBussinessName;
                //    existingSupplier.Name = s.Fullname;
                //    existingSupplier.Address = s.Address;
                //    existingSupplier.City = s.City;
                //    existingSupplier.State = s.State;
                //    existingSupplier.Zipcode = s.Zipcode;
                //    existingSupplier.ExpectedDays = s.ExpertedDays;
                //    existingSupplier.EmailId = s.Email;
                //    existingSupplier.Phone = s.CellPhone;
                //    existingSupplier.SACcode = s.SACCode;
                //    existingSupplier.GSTno = s.GSTNumber;
                //    existingSupplier.IsActive = s.Isactive;
                //    existingSupplier.ModifiedBy = s.ModifiedBy;
                //    existingSupplier.ModifiedOn = DateTime.Now;
                //    ctx.SaveChanges();
                //}
                //else
                //{
                //    return NotFound();
                //}
            }

            return Ok();
        }
        [HttpDelete]
        [ActionName("DeleteSupplierData")]
        public IHttpActionResult DeleteSupplier(int id)
        {
            using (var ctx = new InventoryManagementEntities())
            {

               // var student = ctx.IM_SUPPLIER
               //.Where(s => s.ID == id)
               //.FirstOrDefault();

               // ctx.Entry(student).State = System.Data.Entity.EntityState.Deleted;
               // ctx.SaveChanges();
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
            //    IM_SUPPLIER s = hhh.IM_SUPPLIER.Where(s1 => s1.ID == id).FirstOrDefault();
            //    if (s != null)
            //    {
            //        Supplier.Id = s.ID;
            //        Supplier.CoreTypeId = s.CoreTypeId.Value;
            //        Supplier.Plantid = s.PlantId.Value;
            //        Supplier.SupplierBussinessName = s.BusinessName;
            //        //Supplier.Fullname = s.Name;
            //        Supplier.Address = s.Address;
            //        Supplier.City = s.City;
            //        Supplier.State = s.State;
            //        Supplier.Zipcode = s.Zipcode;
            //        Supplier.ExpertedDays = s.ExpectedDays;
            //        Supplier.Email = s.EmailId;
            //        Supplier.CellPhone = s.Phone;
            //        Supplier.SACCode = s.SACcode;
            //        Supplier.GSTNumber = s.GSTno;
            //        Supplier.Isactive = s.IsActive.Value;
            //        Supplier.ModifiedBy = s.ModifiedBy;
            //        Supplier.ModifiedOn = DateTime.Now;
            //    }
            }
            //if (Supplier.Id == 0)
            //    return NotFound();
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
                    var Masterdata = ctx.MasterDatas.Where(s1 => s1.Id == s.Id)
                                                      .FirstOrDefault<MasterData>();
                    if (Masterdata != null)
                    {
                        Masterdata.Id = s.Id;
                        Masterdata.MasterName = s.MasterName;
                        Masterdata.Descrption = s.Description;
                        Masterdata.Isactive = s.Isactive;
                        Masterdata.ModifiedBy = s.ModifiedBy;
                        Masterdata.ModifiedDate = DateTime.Now;
                        //ctx.SaveChanges();
                    }


                }
                else
                {
                    ctx.MasterDatas.Add(new MasterData()
                    {
                        Id = s.Id,
                        MasterName = s.MasterName,
                        Descrption = s.Description,
                        Type = s.Type.ToString(),
                        Isactive = s.Isactive,
                        CreatedBy = s.CreatedBy,
                        CreatedDate = DateTime.Now,
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

                var masterdata = ctx.MasterDatas
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
            List<MasterData> imMaster = new List<MasterData>();
            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                imMaster = hhh.MasterDatas.ToList();
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
                            ModifiedOn = item.ModifiedDate,
                            CreatedBy = item.CreatedBy,
                            CreatedOn = item.CreatedDate
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
                            ModifiedOn = item.ModifiedDate,
                            CreatedBy = item.CreatedBy,
                            CreatedOn = item.CreatedDate
                        }).ToList();
                    }
                }
            }
            return MasterBytypelist;
        }
    }
}