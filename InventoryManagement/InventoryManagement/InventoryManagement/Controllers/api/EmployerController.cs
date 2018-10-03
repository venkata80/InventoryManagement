using InventoryManagement.EF;
using InventoryManagement.Enums;
using InventoryManagement.Models;
using InventoryManagement.Security;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
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
        [ActionName("GetEmployers")]
        public IHttpActionResult GetEmployer(bool? ActiveFL = null)
        {
            IList<EmployerDTO> employers = null;

            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                using (var transaction = hhh.Database.BeginTransaction())
                {
                    try
                    {
                        if (ActiveFL == null)
                            employers = SetEmployerData(hhh.Employers.AsEnumerable());
                        else
                        {
                            var filteredemployers = hhh.Employers.Where(c => c.User.ActiveFL == ActiveFL);
                            if (filteredemployers != null && filteredemployers.Any())
                                employers = SetEmployerData(filteredemployers.AsEnumerable());
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
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
                using (var transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        if (s.Id == Guid.Empty)
                        {
                            UserDTO user = new UserController().CreateUser(new UserDTO { Id = s.Id, Email = s.Email, FirstName = s.FirstName, LastName = s.LastName, MiddleName = s.MiddleName, Role = s.Role, Isactive = true });

                            Guid addressid = SaveAddress(s.Address, user.Id);

                            if (addressid != Guid.Empty)
                                employerid = SaveEmployerDetails(s, user.Id, user.Id, addressid);

                            if (employerid != Guid.Empty && s.SendMailFL)
                                SendRegistrationMail(s);
                        }
                        else
                        {
                            var existinguser = ctx.Users.FirstOrDefault(c => c.ID == s.Id);
                            if (existinguser != null)
                            {
                                employerid = s.Id;
                                //existinguser = s;                            
                                existinguser.FirstName = s.FirstName;
                                existinguser.LastName = s.LastName;
                                existinguser.MiddleName = s.MiddleName;
                                existinguser.ActiveFL = s.Isactive;
                                existinguser.ModifiedBy = s.Id;
                                existinguser.ModifiedDate = DateTime.UtcNow;
                                ctx.Entry(existinguser).State = System.Data.Entity.EntityState.Modified;
                                ctx.SaveChanges();
                                SaveAddress(s.Address, existinguser.ModifiedBy);
                                employerid = SaveEmployerDetails(s, s.Id, s.ModifiedBy, s.Address.Id);
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (employerid == Guid.Empty)
                        {
                            DeleteUserDetails(s.Email);
                        }
                    }
                }
            }

            //  if (employerid == Guid.Empty)
            //  {
            //      DeleteUserDetails(s.Email);
            //  }
            return Ok();
        }

        Guid SaveAddress(AddressDTO addressDTO, Guid createdby)
        {
            Guid? addressid = addressDTO.Id == Guid.Empty ? Guid.NewGuid() : addressDTO.Id;
            try
            {
                Address address;
                using (var addressctx = new InventoryManagementEntities())
                {
                    if (addressDTO.Id == Guid.Empty)
                    {
                        address = new Address();
                        address.ID = addressid.Value;
                        address.Address1 = addressDTO.Address;
                        address.City = addressDTO.City;
                        address.State = addressDTO.State;
                        address.Zipcode = addressDTO.Zipcode;
                        if (addressDTO.Id == Guid.Empty)
                        {
                            address.CreatedBy = createdby;
                            address.CreatedDate = DateTime.UtcNow;
                        }
                        address.ModifiedBy = createdby;
                        if (addressDTO.Id != Guid.Empty)
                            address.ModifiedDate = DateTime.UtcNow;

                        address.ActiveFL = true;
                        addressctx.Addresses.Add(address);
                        addressctx.SaveChanges();

                    }
                    else
                    {
                        address = addressctx.Addresses.Where(c => c.ID == addressid).FirstOrDefault();
                        if (address != null)
                        {
                            address.ID = addressDTO.Id;
                            address.Address1 = addressDTO.Address;
                            address.City = addressDTO.City;
                            address.State = addressDTO.State;
                            address.Zipcode = addressDTO.Zipcode;
                            address.ModifiedBy = createdby;
                            address.ModifiedDate = DateTime.UtcNow;
                            address.ActiveFL = true;
                            addressctx.Entry(address).State = System.Data.Entity.EntityState.Modified;
                            addressctx.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
            return addressid.Value;
        }

        Guid SaveEmployerDetails(EmployerDTO employerdto, Guid userid, Guid createdby, Guid addressid)
        {
            try
            {
                using (var employerctx = new InventoryManagementEntities())
                {
                    Employer emp;
                    if (employerdto.Id == Guid.Empty)
                    {
                        emp = new Employer();
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
                            emp.ModifiedBy = createdby;
                            emp.CreatedDate = DateTime.UtcNow;
                        }

                        employerctx.Employers.Add(emp);
                        employerctx.SaveChanges();
                    }
                    else
                    {
                        emp = employerctx.Employers.Where(e => e.ID == userid).FirstOrDefault();
                        if (emp != null)
                        {
                            //emp.ID = userid;
                            emp.Designation = employerdto.Designation;
                            emp.DOB = employerdto.Dateofbirth;
                            emp.Gender = employerdto.gender;
                            emp.ResPhone = employerdto.ResPhone;
                            emp.CellPhone = employerdto.CellPhone;
                            emp.JoinDate = employerdto.JoinDate.HasValue ? employerdto.JoinDate.Value.Date : DateTime.MinValue;
                            emp.RelievedDate = employerdto.Relieved.HasValue ? employerdto.Relieved.Value.Date : DateTime.MinValue;
                            emp.ModifiedBy = createdby;
                            emp.ModifiedDate = DateTime.UtcNow;
                            employerctx.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                            employerctx.SaveChanges();
                        }
                    }
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
                        if (user != null)
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

        IList<EmployerDTO> SetEmployerData(IEnumerable<Employer> employers)
        {
            return employers.Select(s => new EmployerDTO()
            {
                Id = s.ID,
                LastName = s.User.LastName,
                FirstName = s.User.FirstName,
                MiddleName = s.User.MiddleName,
                Designation = s.Designation,
                Dateofbirth = s.DOB,
                gender = s.Gender,
                Email = s.User.Email,
                ResPhone = s.ResPhone,
                CellPhone = s.CellPhone,
                AddressID = s.AddressID,
                Address = new AddressDTO
                {
                    Id = s.Address.ID,
                    Address = s.Address.Address1,
                    City = s.Address.City,
                    State = s.Address.State,
                    Zipcode = s.Address.Zipcode,
                },
                JoinDate = s.JoinDate,
                Relieved = s.RelievedDate,
                Isactive = s.User.ActiveFL.Value
            }).OrderBy(c => c.JoinDate).ToList();
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
                if (user != null)
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

        [HttpGet]
        public IHttpActionResult EnableEmployer(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            using (var ctx = new InventoryManagementEntities())
            {
                var user = ctx.Users.FirstOrDefault(c => c.ID == id);
                if (user != null)
                {
                    user.ActiveFL = true;
                    ctx.SaveChanges();
                }
            }
            return Ok();
        }

        void SendRegistrationMail(EmployerDTO employer)
        {
            string RegistrationTemplate = Resources.Communication.MailTemplates.NewRegistration_Body;
            if (!string.IsNullOrWhiteSpace(RegistrationTemplate))
            {
                RegistrationTemplate = RegistrationTemplate.Replace("[[[Name]]]", employer.Fullname);
                RegistrationTemplate = RegistrationTemplate.Replace("[[[UserName]]]", employer.Email);

                EFMembershipProvider membershipprovider = new EFMembershipProvider();
                membershipprovider.Initialize("SqlProvider", new NameValueCollection());

                string password = membershipprovider.GetPassword(employer.Email, "bye");

                RegistrationTemplate = RegistrationTemplate.Replace("[[[Password]]]", password);
                RegistrationTemplate = RegistrationTemplate.Replace("[[[Link]]]", Resources.Communication.MailTemplates.LoginLink);


                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(new MailAddress(employer.Email));
                mailMessage.Subject = Resources.Communication.MailTemplates.NewRegistration_Subject;
                mailMessage.Body = RegistrationTemplate;
                mailMessage.From = new MailAddress(Resources.Communication.MailTemplates.SmtpClient_UserName);
                mailMessage.IsBodyHtml = true;

                SendMail(mailMessage);
            }
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

                SUPPLIER = SetSuppliersData(hhh.Suppliers.AsEnumerable());

            }
            if (SUPPLIER.Count == 0)
                return NotFound();
            return Ok(SUPPLIER);
        }

        IList<SuppliersDTO> SetSuppliersData(IEnumerable<Supplier> Supplierlist)
        {
            return Supplierlist.Select(s => new SuppliersDTO()
            {
                Id = s.ID,
                Email = s.User.Email,
                SupplierBussinessName = s.BusinessName,
                FirstName = s.Name,
                Plantid = s.PlantId.HasValue ? s.PlantId.Value : int.MinValue,
                CoreTypeId = s.CoreTypeId.HasValue ? s.CoreTypeId.Value : int.MinValue,
                ExpertedDays = s.ExpectedDays,
                SACCode = s.SACcode,
                GSTNumber = s.GSTno,
                //LastName = s.User.LastName,
                //FirstName = s.User.FirstName,
                //MiddleName = s.User.MiddleName,
                //Designation = s.Designation,
                //Dateofbirth = s.DOB,
                //gender = s.Gender,
                //Email = s.User.Email,
                ResPhone = s.Phone,
                //CellPhone = s.CellPhone,
                // AddressID = s.AddressID,
                Address = new AddressDTO
                {
                    Id = s.Address.ID,
                    Address = s.Address.Address1,
                    City = s.Address.City,
                    State = s.Address.State,
                    Zipcode = s.Address.Zipcode,
                },
                //JoinDate = s.JoinDate,
                //Relieved = s.RelievedDate,
                Isactive = s.User.ActiveFL.Value
            }).ToList();
        }

        [HttpPost]
        [ActionName("InsertSupplierData")]
        public IHttpActionResult PostNewSuppliers([FromBody]SuppliersDTO s)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest("Not a valid model");

            Guid employerid = Guid.Empty;
            using (var ctx = new InventoryManagementEntities())
            {
                try
                {
                    if (s.Id == Guid.Empty)
                    {
                        UserDTO user = new UserController().CreateUser(new UserDTO { Id = s.Id, Email = s.Email, FirstName = s.FirstName, LastName = s.LastName, MiddleName = s.MiddleName, Role = s.Role, Isactive = true });

                        Guid addressid = SaveAddress(s.Address, user.Id);

                        if (addressid != Guid.Empty)
                            employerid = SaveSupplierDetails(s, user.Id, user.Id, addressid);

                        //if (employerid != Guid.Empty)
                        //    SendRegistrationMail(s);
                    }
                    else
                    {
                        var existinguser = ctx.Users.FirstOrDefault(c => c.ID == s.Id);
                        if (existinguser != null)
                        {
                            employerid = s.Id;
                            existinguser.ActiveFL = s.Isactive;
                            existinguser.ModifiedBy = s.Id;
                            existinguser.ModifiedDate = DateTime.UtcNow;
                            ctx.Entry(existinguser).State = System.Data.Entity.EntityState.Modified;
                            ctx.SaveChanges();
                            SaveAddress(s.Address, existinguser.ModifiedBy);
                            employerid = SaveSupplierDetails(s, s.Id, s.ModifiedBy, s.Address.Id);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            if (employerid == Guid.Empty)
            {
                DeleteUserDetails(s.Email);
            }
            return Ok();
        }

        Guid SaveSupplierDetails(SuppliersDTO supplierdt, Guid userid, Guid createdby, Guid addressid)
        {
            try
            {
                using (var supplierctx = new InventoryManagementEntities())
                {
                    Supplier emp;
                    if (supplierdt.Id == Guid.Empty)
                    {
                        emp = new Supplier();
                        emp.ID = userid;
                        emp.BusinessName = supplierdt.SupplierBussinessName;
                        emp.PlantId = supplierdt.Plantid;
                        emp.CoreTypeId = supplierdt.CoreTypeId;
                        emp.ExpectedDays = supplierdt.ExpertedDays;
                        emp.Phone = supplierdt.CellPhone;
                        emp.GSTno = supplierdt.GSTNumber;
                        emp.AddressID = addressid;
                        emp.SACcode = supplierdt.SACCode;
                        if (supplierdt.Id == Guid.Empty)
                        {
                            emp.CreatedBy = createdby;
                            emp.ModifiedBy = createdby;
                            emp.CreatedOn = DateTime.UtcNow;
                        }

                        supplierctx.Suppliers.Add(emp);
                        supplierctx.SaveChanges();
                    }
                    else
                    {
                        emp = supplierctx.Suppliers.Where(e => e.ID == userid).FirstOrDefault();
                        if (emp != null)
                        {
                            emp.BusinessName = supplierdt.SupplierBussinessName;
                            emp.PlantId = supplierdt.Plantid;
                            emp.CoreTypeId = supplierdt.CoreTypeId;
                            emp.ExpectedDays = supplierdt.ExpertedDays;
                            emp.Phone = supplierdt.CellPhone;
                            emp.GSTno = supplierdt.GSTNumber;
                            emp.SACcode = supplierdt.SACCode;
                            emp.ModifiedBy = createdby;
                            emp.ModifiedOn = DateTime.UtcNow;
                            supplierctx.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                            supplierctx.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
            return userid;
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
        public IHttpActionResult GetSupplierBy(Guid id)
        {
            SuppliersDTO Supplier = new SuppliersDTO();
            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                Supplier s = hhh.Suppliers.Where(s1 => s1.ID == id).FirstOrDefault();
                if (s != null)
                {
                    Supplier.Id = s.ID;
                    Supplier.CoreTypeId = s.CoreTypeId.HasValue ? s.CoreTypeId.Value : int.MinValue;
                    Supplier.Plantid = s.PlantId.HasValue ? s.PlantId.Value : int.MinValue;
                    Supplier.SupplierBussinessName = s.BusinessName;
                    //Supplier.Fullname = s.Name;
                    Supplier.Address = new AddressDTO
                    {
                        Address = s.Address.Address1,
                        City = s.Address.City,
                        State = s.Address.State,
                        Zipcode = s.Address.Zipcode,
                        Id = s.Address.ID
                    };
                    Supplier.ExpertedDays = s.ExpectedDays;
                    Supplier.Email = s.User.Email;
                    Supplier.CellPhone = s.Phone;
                    Supplier.SACCode = s.SACcode;
                    Supplier.GSTNumber = s.GSTno;
                    Supplier.Isactive = Convert.ToBoolean(s.User.ActiveFL);
                }
            }
            if (Supplier.Id == Guid.Empty)
                return NotFound();
            return Ok(Supplier);
        }
        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        #endregion

        public IHttpActionResult GetSuplirespricelist(int? CoreItemFL = null)
        {
            IList<SupplierPriceListDTO> productDTO = null;

            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                if (CoreItemFL == null)
                    productDTO = SetSupplierpricelistData(hhh.SupplierPriceLists.AsEnumerable());
                else
                {
                    var filteredemployers = hhh.SupplierPriceLists.Where(c => c.Type == CoreItemFL && c.IsActive == true);
                    if (filteredemployers != null && filteredemployers.Any())
                        productDTO = SetSupplierpricelistData(filteredemployers.AsEnumerable());
                }
            }
            return Ok(productDTO);
        }

        IList<SupplierPriceListDTO> SetSupplierpricelistData(IEnumerable<SupplierPriceList> suppliers)
        {
            return suppliers.Select(c => new SupplierPriceListDTO
            {
                Id = c.SupplierPLId,
                SupplierId = (c.SupplierId == null ? Guid.Empty : c.SupplierId.Value),
                ProductId = (c.ProductId == null ? Guid.Empty : c.ProductId.Value),
                Brand = c.BrandId,
                Type = c.Type,
                FreezingType = c.FreezingId,
                ProductForm = c.ProductformId,
                Variety = c.VarietyId,
                Specie = c.SpiceId,
                PackingType = c.PackingId,
                PackingCount = c.PackingUnits.Value,
                Grade = c.GradeId,
                Soaked = c.Socked,
                ExpectedDays = c.ExpertedDays,
                VenderUnits = c.SupplierUnits,
                IsActive = c.IsActive.Value,
                CreatedBy = c.CreatedBy.Value,
                CreatedOn = c.CreatedOn,
                ModifiedBy = c.ModifiedBy.Value
            }).ToList();
        }

        [HttpPost]
        public IHttpActionResult SaveSuppliersPriceList(SupplierPriceListDTO product)
        {
            using (var productctx = new InventoryManagementEntities())
            {
                try
                {
                    SupplierPriceList pro1 = new SupplierPriceList();
                    if(product.Id==Guid.Empty)
                        productctx.SupplierPriceLists.Add(SetSupplier(pro1,product));
                    else
                    {
                        pro1 = productctx.SupplierPriceLists.Where(c => c.SupplierPLId == product.Id).FirstOrDefault();
                        SetSupplier(pro1, product);
                    }                       

                    if (product.Id != Guid.Empty)
                        productctx.Entry(pro1).State = System.Data.Entity.EntityState.Modified;
                    else
                        productctx.Entry(pro1).State = System.Data.Entity.EntityState.Added;

                    productctx.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
            return Ok();
        }

        SupplierPriceList SetSupplier(SupplierPriceList pro, SupplierPriceListDTO product)
        {
            int? Nullablevalue = null;
            if(product.Id ==Guid.Empty)
             pro = new SupplierPriceList();
            pro.SupplierPLId = product.Id == Guid.Empty ? Guid.NewGuid() : product.Id;
            pro.ProductId = product.ProductId == Guid.Empty ? Guid.NewGuid() : product.ProductId;
           pro.SupplierId = product.SupplierId == Guid.Empty ? Guid.NewGuid() : product.SupplierId;
            pro.Type = product.Type;
            pro.BrandId = Convert.ToInt64(product.Brand) > long.MinValue ? product.Brand : Nullablevalue;
            pro.ProductformId = Convert.ToInt64(product.ProductForm) > long.MinValue ? product.ProductForm : Nullablevalue;
            pro.VarietyId = Convert.ToInt64(product.Variety) > long.MinValue ? product.Variety : Nullablevalue;
            pro.SpiceId = Convert.ToInt64(product.Specie) > long.MinValue ? product.Specie : Nullablevalue;
            pro.FreezingId = Convert.ToInt64(product.FreezingType) > long.MinValue ? product.FreezingType : Nullablevalue;
            pro.PackingId = Convert.ToInt64(product.PackingType) > long.MinValue ? product.PackingType : Nullablevalue;
            pro.PackingUnits = Convert.ToInt64(product.PackingCount) > long.MinValue ? Convert.ToInt32(product.PackingCount) : Nullablevalue;
            pro.ProductTypeId = Convert.ToInt64(product.ProductType) > long.MinValue ? Convert.ToInt32(product.ProductType) : Nullablevalue;
            pro.GradeId = Convert.ToInt64(product.Grade);
            pro.Socked = Convert.ToInt64(product.Soaked) > long.MinValue ? product.Soaked : Nullablevalue;
            pro.SupplierUnits = Convert.ToInt64(product.VenderUnits) > long.MinValue ? product.VenderUnits : Nullablevalue;
            pro.ExpertedDays = product.ExpectedDays;
            pro.CategoryType= Convert.ToInt64(product.Category) > long.MinValue ? Convert.ToInt32(product.Category) : Nullablevalue; 
            pro.IsActive = true;
            if (product.Id == Guid.Empty)
            {
                pro.CreatedBy = product.CreatedBy;
                pro.CreatedOn = DateTime.UtcNow;
            }
            else
                pro.ModifiedOn = DateTime.UtcNow;

            pro.ModifiedBy = product.ModifiedBy;

            return pro;
        }


        #region Master Data

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
                        Masterdata.UnitsID = s.UnitId;
                        Masterdata.Isactive = s.Isactive;
                        Masterdata.ModifiedBy = s.ModifiedBy;
                        Masterdata.ModifiedDate = DateTime.Now;
                    }
                }
                else
                {
                    ctx.MasterDatas.Add(new MasterData()
                    {
                        Id = s.Id,
                        MasterName = s.MasterName,
                        Descrption = s.Description,
                        UnitsID = s.UnitId,
                        Type = s.Type.ToString(),
                        Isactive = s.Isactive,
                        CreatedBy = s.CreatedBy,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = s.ModifiedBy,
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

        //UnitsList
        [HttpGet]
        [ActionName("GetUnitsData")]
        public IHttpActionResult GetUnitslist()
        {
            List<UnitsData> Masterunitlist = new List<UnitsData>();
            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {

                Masterunitlist = hhh.Units.Select(item => new UnitsData()
                {
                    UnitId = item.Id,
                    Unitname = item.Name,
                    UnitDesc = item.Descrption,

                }).ToList();


            }
            return Ok(Masterunitlist);

        }

        List<MasterDataDTO> MaterDataList(long id = 0)
        {
            List<MasterDataDTO> MasterBytypelist = new List<MasterDataDTO>();
            List<MasterData> imMaster = new List<MasterData>();
            List<Unit> imUnitdata = new List<Unit>();
            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                imMaster = hhh.MasterDatas.ToList();
                imUnitdata = hhh.Units.ToList();
                if (imMaster != null && imMaster.Any())
                {
                    if (id == 0)
                    {

                        var ff = from MasterData in imMaster
                                 join Unit in imUnitdata on MasterData.UnitsID equals Unit.Id into gj
                                 from master1 in gj.DefaultIfEmpty()
                                 select new
                                 {
                                     Id = MasterData.Id,
                                     MasterName = MasterData.MasterName,
                                     Description = MasterData.Descrption,
                                     Type = MasterData.Type,
                                     Isactive = MasterData.Isactive.Value,
                                     UnitId = (MasterData.UnitsID == null ? 0 : MasterData.UnitsID.Value),
                                     ModifiedBy = MasterData.ModifiedBy,
                                     ModifiedOn = MasterData.ModifiedDate,
                                     CreatedBy = MasterData.CreatedBy,
                                     CreatedOn = MasterData.CreatedDate,
                                     Unitname = (master1 == null ? "" : master1.Name),
                                 };
                        foreach (var item in ff)
                        {
                            MasterDataDTO mdto = new MasterDataDTO();
                            mdto.Id = item.Id;
                            mdto.MasterName = item.MasterName;
                            mdto.Description = item.Description;
                            mdto.Type = (MasterDataType)Enum.Parse(typeof(MasterDataType), item.Type);
                            mdto.Isactive = item.Isactive;
                            mdto.UnitId = item.UnitId;
                            mdto.ModifiedBy = item.ModifiedBy;
                            mdto.ModifiedOn = item.ModifiedOn;
                            mdto.CreatedBy = item.CreatedBy;
                            mdto.CreatedOn = item.CreatedOn;
                            mdto.Unitname = item.Unitname;
                            MasterBytypelist.Add(mdto);
                        }
                        // MasterBytypelist = (List<MasterDataDTO>)ff;
                        //MasterBytypelist = imMaster.Select(item => new MasterDataDTO()
                        //{
                        //    Id = item.Id,
                        //    MasterName = item.MasterName,
                        //    Description = item.Descrption,
                        //    Type = (MasterDataType)Enum.Parse(typeof(MasterDataType), item.Type),
                        //    Isactive = item.Isactive.Value,
                        //    UnitId=(item.UnitsID == null ? 0 : item.UnitsID.Value),
                        //    ModifiedBy = item.ModifiedBy,
                        //    ModifiedOn = item.ModifiedDate,
                        //    CreatedBy = item.CreatedBy,
                        //    CreatedOn = item.CreatedDate
                        //}).ToList();
                    }
                    else
                    {
                        //MasterBytypelist = imMaster.Where(c => c.Id == id)?.Select(item => new MasterDataDTO()
                        //{
                        //    Id = item.Id,
                        //    MasterName = item.MasterName,
                        //    Description = item.Descrption,
                        //    Type = (MasterDataType)Enum.Parse(typeof(MasterDataType), item.Type),
                        //    Isactive = item.Isactive.Value,
                        //    UnitId = (item.UnitsID == null ? 0 : item.UnitsID.Value),
                        //    ModifiedBy = item.ModifiedBy,
                        //    ModifiedOn = item.ModifiedDate,
                        //    CreatedBy = item.CreatedBy,
                        //    CreatedOn = item.CreatedDate
                        //}).ToList();
                        var ff = from MasterData in imMaster
                                 join Unit in imUnitdata on MasterData.UnitsID equals Unit.Id into gj
                                 from master1 in gj.DefaultIfEmpty()
                                 select new
                                 {
                                     Id = MasterData.Id,
                                     MasterName = MasterData.MasterName,
                                     Description = MasterData.Descrption,
                                     Type = MasterData.Type,
                                     Isactive = MasterData.Isactive.Value,
                                     UnitId = (MasterData.UnitsID == null ? 0 : MasterData.UnitsID.Value),
                                     ModifiedBy = MasterData.ModifiedBy,
                                     ModifiedOn = MasterData.ModifiedDate,
                                     CreatedBy = MasterData.CreatedBy,
                                     CreatedOn = MasterData.CreatedDate,
                                     Unitname = (master1 == null ? "" : master1.Name),
                                 };
                        foreach (var item in ff)
                        {
                            MasterDataDTO mdto = new MasterDataDTO();
                            if (item.Id == id)
                            {
                                mdto.Id = item.Id;
                                mdto.MasterName = item.MasterName;
                                mdto.Description = item.Description;
                                mdto.Type = (MasterDataType)Enum.Parse(typeof(MasterDataType), item.Type);
                                mdto.Isactive = item.Isactive;
                                mdto.UnitId = item.UnitId;
                                mdto.ModifiedBy = item.ModifiedBy;
                                mdto.ModifiedOn = item.ModifiedOn;
                                mdto.CreatedBy = item.CreatedBy;
                                mdto.CreatedOn = item.CreatedOn;
                                mdto.Unitname = item.Unitname;
                                MasterBytypelist.Add(mdto);
                            }
                        }
                    }
                }
            }
            return MasterBytypelist;
        }

        #endregion

        #region Products

        public IHttpActionResult GetProducts(int? CoreItemFL = null)
        {
            IList<ProductDTO> productDTO = null;

            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                if (CoreItemFL == null)
                    productDTO = SetProductData(hhh.Products.AsEnumerable());
                else
                {
                    var filteredemployers = hhh.Products.Where(c => c.Type == CoreItemFL && c.Isactive == true);
                    if (filteredemployers != null && filteredemployers.Any())
                        productDTO = SetProductData(filteredemployers.AsEnumerable());
                }
            }
            return Ok(productDTO);
        }

        public IHttpActionResult GetProduct(Guid id)
        {
            ProductDTO product = null;
            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                var filteredemployers = hhh.Products.Where(c => c.ID == id);
                if (filteredemployers != null && filteredemployers.Any())
                {
                    product = SetProductData(filteredemployers.AsEnumerable()).FirstOrDefault();
                }
            }
            return Ok(product);
        }

        IList<ProductDTO> SetProductData(IEnumerable<Product> products)
        {
            return products.Select(c => new ProductDTO
            {
                Id = c.ID,
                Type = c.Type,
                //Name = c.Name,
                Description = c.Description,
                ShortCode = c.ShortCode,
                Brand = c.Brand,
                ProductForm = c.ProductForm,
                Variety = c.Variety,
                Specie = c.Specie,
                FreezingType = c.FreezingType,
                PackingType = c.PackingType,
                Quantity = c.Quantity,
                PackingStyle = c.PackingStyle,
                Grade = c.Grade,
                Soaked = c.Soaked,
                Ply = c.Ply,
                Print = c.PrintType,
                Top =c.TopType,
                Dimensions = c.Dimensions,
                NetWeight=c.NetWeight,
                ThresholdLimit = Convert.ToInt32(c.ThresholdLimit).ToString(),
                Category = c.Catergory,
                Unit = c.Unit,
                CreatedBy=c.CreatedBy,
                CreatedOn=c.CreatedDate,
                ModifiedBy=c.ModifiedBy,
                Isactive=c.Isactive.Value,
            }).ToList();
        }
       
        [HttpPost]
        public IHttpActionResult SaveProduct(ProductDTO product)
        {
            using (var productctx = new InventoryManagementEntities())
            {
                try
                {
                    Product pro1 = new Product();

                    if (product.Id == null)
                    {
                        productctx.Products.Add(SetProduct(pro1, product));
                    }
                    else
                    {
                        pro1 = productctx.Products.Where(c => c.ID == product.Id).FirstOrDefault();
                        SetProduct(pro1, product);
                    }

                    if (product.Id != Guid.Empty)
                        productctx.Entry(pro1).State = System.Data.Entity.EntityState.Modified;
                    else
                        productctx.Entry(pro1).State = System.Data.Entity.EntityState.Added;

                    productctx.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
            return Ok();
        }

        Product SetProduct(Product pro, ProductDTO product)
        {
            int? Nullablevalue = null;
            if(product.Id ==Guid.Empty)
             pro = new Product();
            pro.ID = product.Id == Guid.Empty ? Guid.NewGuid() : product.Id;
            pro.Name = product.Name;
            pro.Description = product.Description;
            pro.Type = product.Type;
            pro.ShortCode = product.ShortCode;
            pro.Brand = Convert.ToInt64(product.Brand) > long.MinValue ? product.Brand : Nullablevalue;
            pro.ProductForm = Convert.ToInt64(product.ProductForm) > long.MinValue ? product.ProductForm : Nullablevalue;
            pro.Variety = Convert.ToInt64(product.Variety) > long.MinValue ? product.Variety : Nullablevalue;
            pro.Specie = Convert.ToInt64(product.Specie) > long.MinValue ? product.Specie : Nullablevalue;
            pro.FreezingType = Convert.ToInt64(product.FreezingType) > long.MinValue ? product.FreezingType : Nullablevalue;
            pro.PackingType = Convert.ToInt64(product.PackingType) > long.MinValue ? product.PackingType : Nullablevalue;
            pro.Quantity = Convert.ToInt64(product.Quantity) > long.MinValue ? Convert.ToInt32(product.Quantity) : Nullablevalue;
            pro.PackingStyle = Convert.ToInt64(product.PackingStyle) > long.MinValue ? Convert.ToInt32(product.PackingStyle) : Nullablevalue;
            pro.Grade = Convert.ToInt64(product.Grade);
            pro.Soaked = Convert.ToInt64(product.Soaked) > long.MinValue ? Convert.ToInt32(product.Soaked) : Nullablevalue;
            pro.Ply = Convert.ToInt64(product.Ply) > long.MinValue ? Convert.ToInt32(product.Ply) : Nullablevalue;
            pro.PrintType = Convert.ToInt64(product.Print) > long.MinValue ? Convert.ToInt32(product.Print) : Nullablevalue;
            pro.TopType = Convert.ToInt64(product.Top) > long.MinValue ? Convert.ToInt32(product.Top) : Nullablevalue;
            pro.Dimensions = product.Dimensions;
            pro.NetWeight = product.NetWeight;
            pro.ThresholdLimit = !string.IsNullOrWhiteSpace(product.ThresholdLimit) ? Convert.ToInt32(product.ThresholdLimit) : Nullablevalue;
            pro.Catergory = Convert.ToInt64(product.Category) > long.MinValue ? product.Category : Nullablevalue;
            pro.Unit = Convert.ToInt64(product.Unit) > long.MinValue ? product.Unit : Nullablevalue;
            pro.Isactive = product.Isactive;
            pro.CreatedBy = product.CreatedBy;
            if (product.Id == Guid.Empty)
            {
                pro.CreatedBy = product.CreatedBy;
                pro.CreatedDate = DateTime.UtcNow;
            }
            else
                pro.ModifiedDate = DateTime.UtcNow;

            pro.ModifiedBy = product.ModifiedBy;

            return pro;
        }

        [HttpDelete]
        [ActionName("InactiveProductData")]
        public IHttpActionResult InactiveProduct(Guid id)
        {           
            using (var ctx = new InventoryManagementEntities())
            {
                var user = ctx.Products.FirstOrDefault(c => c.ID == id);
                if (user != null)
                {
                    user.Isactive = false;
                    ctx.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();
                }

            }

            return Ok();
        }

        [HttpGet]
        [ActionName("GetProductTaxesData")]
        public IHttpActionResult GetProductsTaxes()
        {
            IList<ProductTypeTaxesDTO> productDTO = null;
            
            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
               
                    var filteredemployers = hhh.ProductTypeTaxes.Where(c => c.Isactive == true);
                    if (filteredemployers != null && filteredemployers.Any())
                        productDTO = SetProductData(filteredemployers.AsEnumerable());
                
            }
            return Ok(productDTO);
        }

        IList<ProductTypeTaxesDTO> SetProductData(IEnumerable<ProductTypeTax> products)
        {
            return products.Select(c => new ProductTypeTaxesDTO
            {
                ProductTaxeId = c.Id,
                ProductTypeId = c.ProductTypeId,
                SGST = c.SGST,
                CGST = c.CGST,
                IGST = c.IGST,
                AffectiveFrom = c.AffectiveFrom,
                AffectiveTo = c.AffectiveTo,                
                CreatedBy = c.CreatedBy.Value,
                CreatedOn = c.CreatedOn,
                ModifiedBy = c.ModifiedBy.Value,
                Isactive = c.Isactive.Value,
            }).ToList();
        }
        [HttpPost]
        public IHttpActionResult SaveProductTaxes(ProductTypeTaxesDTO product)
        {
            using (var productctx = new InventoryManagementEntities())
            {
                try
                {
                    ProductTypeTax pro1=new ProductTypeTax();

                    if (product.ProductTaxeId ==null || product.ProductTaxeId == 0)
                    {
                        // SetProductTaxes(pro,product);
                        productctx.ProductTypeTaxes.Add(SetProductTaxes(pro1, product));
                    }
                    else
                    {
                        pro1= productctx.ProductTypeTaxes.Where(c => c.Id == product.ProductTaxeId).FirstOrDefault();
                        SetProductTaxes(pro1, product);
                    }
                      

                    if (product.ProductTaxeId == null ||  product.ProductTaxeId == 0)
                        productctx.Entry(pro1).State = System.Data.Entity.EntityState.Added;
                    else
                        productctx.Entry(pro1).State = System.Data.Entity.EntityState.Modified;

                    productctx.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
            return Ok();
        }

        ProductTypeTax SetProductTaxes(ProductTypeTax pro, ProductTypeTaxesDTO product)
        {           
            if(product.ProductTaxeId == null || product.ProductTaxeId==0)
            pro = new ProductTypeTax();
           // else

            pro.ProductTypeId = product.ProductTypeId;
            pro.SGST = product.SGST;
            pro.CGST = product.CGST;
            pro.IGST = product.IGST;
            pro.AffectiveFrom = product.AffectiveFrom;
            pro.AffectiveTo = product.AffectiveTo;
            pro.Isactive = product.Isactive;
            if (product.ProductTaxeId == null || product.ProductTaxeId == 0)
            {
                pro.CreatedBy = product.CreatedBy;
                pro.CreatedOn = DateTime.UtcNow;
            }
            else
                pro.ModifiedOn = DateTime.UtcNow;

            pro.ModifiedBy = product.ModifiedBy;

            return pro;
        }

        public IHttpActionResult GetProductTaxes(long? id)
        {
            ProductTypeTaxesDTO product = null;
            using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            {
                var filteredemployers = hhh.ProductTypeTaxes.Where(c => c.Id == id);
                if (filteredemployers != null && filteredemployers.Any())
                {
                    product = SetProductData(filteredemployers.AsEnumerable()).FirstOrDefault();
                }
            }
            return Ok(product);
        }

        [HttpDelete]
        [ActionName("InactiveProductTaxesData")]
        public IHttpActionResult InactiveProductTaxes(long id)
        {
            using (var ctx = new InventoryManagementEntities())
            {
                var user = ctx.ProductTypeTaxes.FirstOrDefault(c => c.Id == id);
                if (user != null)
                {
                    user.Isactive = false;
                    ctx.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();
                }

            }

            return Ok();
        }
        #endregion

        void SendMail(MailMessage Message)
        {
            SmtpClient client = new SmtpClient();
            client.Host = Resources.Communication.MailTemplates.SmtpClient_Host;
            client.Port = Convert.ToInt32(Resources.Communication.MailTemplates.SmtpClient_Port);
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(Resources.Communication.MailTemplates.SmtpClient_UserName, Resources.Communication.MailTemplates.SmtpClient_Password);
            client.Send(Message);
        }
    }
}