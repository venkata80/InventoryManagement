using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventoryManagement.Controllers.api
{
    public class LoginController : ApiController
    {
        [HttpGet]
        [ActionName("GetMemberById")]
        public IHttpActionResult GetMember()
        {
            MemberShipDTO  Member = new MemberShipDTO();

            //using (InventoryManagementEntities hhh = new InventoryManagementEntities())
            //{
            //    students = hhh.IM_Employer.Select(s => new BaseEmployerDTO()
            //    {
            //        Id = s.EMPID,
            //        LastName = s.LastName,
            //        FirstName = s.FirstName,
            //        MiddleName = s.Middlename,
            //        //Fullname=s.FirstName+ ' '+s.Middlename+' '+s.LastName,
            //        Designation = s.Designation,
            //        Dateofbirth = s.DOB,
            //        gender = s.Gender,
            //        Email = s.Email,
            //        ResPhone = s.ResPhone,
            //        CellPhone = s.CellPhone,
            //        Address = s.Address,
            //        City = s.City,
            //        State = s.State,
            //        Zipcode = s.Zipcode,
            //        JoinDate = s.JoinDate,
            //        Relieved = s.RelievedDate,
            //        Isactive = true
            //    }).ToList<BaseEmployerDTO>();
            //    if (students == null)
            //        students = new List<BaseEmployerDTO>();
            //}
            //if (students.Count == 0)
            //    return NotFound();
            return Ok(Member);
        }

    }
}
