using InventoryManagement.Enums;
using InventoryManagement.Models;
using InventoryManagement.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InventoryManagement.Controllers
{
    public class AdminController : Controller
    {
        string value = System.Configuration.ConfigurationManager.AppSettings["urlname"];
        // GET: Admin
        public ActionResult Index()
        {
            return View("Dashboard");
        }

        #region EMPLOYER

        public ActionResult Employers()
        {
            return View("Employer/Employers");
        }

        public ActionResult GetEmployers(bool ActiveFL = true)
        {
            IList<EmployerDTO> emplist = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var responseTask = client.GetAsync("Employer/GetEmployers?ActiveFL=" + ActiveFL);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<EmployerDTO>>();
                    readTask.Wait();

                    emplist = readTask.Result;
                }
            }
            return PartialView("Employer/_Employers", emplist);
        }

        public ActionResult CreateEmployer()
        {
            return View("Employer/CreateEmployer", new EmployerDTO() { Address = new AddressDTO { } });
        }
        [HttpPost]
        public ActionResult CreateEmployer(EmployerDTO student)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(value);
                    var postTask = client.PostAsJsonAsync<EmployerDTO>("Employer/SaveEmployer", student);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ViewEmployer");
                    }

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
                //List<BaseEmployerDTO> emplist = new List<BaseEmployerDTO>();
            }
            return View("Employer/CreateEmployer", student);
        }

        public ActionResult EditEmployer(Guid id)
        {
            EmployerDTO emplist = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var responseTask = client.GetAsync("Employer/GetEmployeeById/" + id);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<EmployerDTO>();
                    readTask.Wait();

                    emplist = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    //  emplist =  IList.Empty<BaseEmployerDTO>();


                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View("Employer/CreateEmployer", emplist);
        }
        [HttpPost]
        public ActionResult EditEmployer(EmployerDTO student)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                student.CreatedBy = ((UserSecurityToken)Session["CurrentUser"]).Id;
                student.ModifiedBy = ((UserSecurityToken)Session["CurrentUser"]).Id;
                var postTask = client.PostAsJsonAsync("Employer/SaveEmployer", student);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("ViewEmployer");
                }
                else //web api sent error response 
                {
                    //log response status here..

                    //  emplist =  IList.Empty<BaseEmployerDTO>();


                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View("Employer/CreateEmployer", student);
        }

        public ActionResult DeleteEmployer(Guid id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var deleteTask = client.DeleteAsync("Employer/DeleteEmployer/" + id);
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("ViewEmployer");
                }
                else //web api sent error response 
                {
                    //log response status here..

                    //  emplist =  IList.Empty<BaseEmployerDTO>();


                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return RedirectToAction("ViewEmployer");
        }

        public ActionResult EnableEmployer(Guid id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var deleteTask = client.GetAsync("Employer/EnableEmployer/" + id);
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("ViewEmployer");
                }
                else //web api sent error response 
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return RedirectToAction("ViewEmployer");
        }
        #endregion

        #region SUPPLIERS

        public ActionResult ViewSuppliers()
        {
            ViewBag.Title = "Home Page";
            IList<SuppliersDTO> emplist = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var responseTask = client.GetAsync("Employer/GetAllSuppliers");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SuppliersDTO>>();
                    readTask.Wait();

                    emplist = readTask.Result;
                    if (emplist == null)
                        emplist = new List<SuppliersDTO>();
                }
                else //web api sent error response 
                {
                    //log response status here..

                    if (emplist == null)
                        emplist = new List<SuppliersDTO>();


                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return View("Supplier/ViewSuppliers", emplist);
        }

        public ActionResult CreateSupplier()
        {
            SuppliersDTO model = new SuppliersDTO();
            return View("Supplier/CreateSupplier", model);
        }
        [HttpPost]
        public ActionResult CreateSupplier(SuppliersDTO student)
        {
            ModelState.Remove("Id");
            ModelState.Remove("FirstName");
            ModelState.Remove("LastName");
            ModelState.Remove("Dateofbirth");
            ModelState.Remove("ResPhone");
            ModelState.Remove("JoinDate");
            ModelState.Remove("Relieved");


            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(value);
                    var postTask = client.PostAsJsonAsync<SuppliersDTO>("Employer/InsertSupplierData", student);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ViewSuppliers");
                    }

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
                //List<BaseEmployerDTO> emplist = new List<BaseEmployerDTO>();
            }
            return View("Supplier/CreateSupplier", student);
        }

        public ActionResult EditSupplier(int id)
        {
            SuppliersDTO emplist = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var responseTask = client.GetAsync("Employer/GetSupplierById/" + id);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<SuppliersDTO>();
                    readTask.Wait();

                    emplist = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    //  emplist =  IList.Empty<BaseEmployerDTO>();


                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View("Supplier/CreateSupplier", emplist);
        }
        [HttpPost]
        public ActionResult EditSupplier(SuppliersDTO student)
        {
            ModelState.Remove("Id");
            ModelState.Remove("FirstName");
            ModelState.Remove("LastName");
            ModelState.Remove("Dateofbirth");
            ModelState.Remove("ResPhone");
            ModelState.Remove("JoinDate");
            ModelState.Remove("Relieved");

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(value);
                    var postTask = client.PutAsJsonAsync("Employer/UpdateSupplierData", student);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ViewSuppliers");
                    }
                    else //web api sent error response 
                    {
                        //log response status here..

                        //  emplist =  IList.Empty<BaseEmployerDTO>();

                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }
            return View("Supplier/CreateSupplier", student);
        }

        public ActionResult DeleteSuppliers(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var deleteTask = client.DeleteAsync("Employer/DeleteSupplierData/" + id);
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("ViewSuppliers");
                }
                else //web api sent error response 
                {
                    //log response status here..

                    //  emplist =  IList.Empty<BaseEmployerDTO>();


                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View();
        }

        #endregion

        #region MASTER DATA

        public ActionResult MasterTab()
        {
            return View("MasterData/MasterTab");
        }
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult MasterDataView(MasterDataType type)
        {
            List<MasterDataDTO> masterlist = new List<MasterDataDTO>();
            //MasterDataType foo = (MasterDataType)Enum.Parse(typeof(MasterDataType), type);

            ViewBag.MaterDataType = type;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var responseTask1 = client.GetAsync("Employer/GetMasterDataBy");
                responseTask1.Wait();
                var result = responseTask1.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<MasterDataDTO>>();
                    readTask.Wait();

                    masterlist = readTask.Result.Where(s => s.Type == type).ToList();
                    if (masterlist == null)
                        masterlist = new List<MasterDataDTO>();
                }
                else //web api sent error response 
                {
                    //log response status here..

                    if (masterlist == null)
                        masterlist = new List<MasterDataDTO>();


                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            TempData["masterDataList"] = masterlist;
            return PartialView("MasterData/_MasterDataList", masterlist);
        }

        [HttpGet]
        public ActionResult MasterDataView1(int id)
        {
            List<MasterDataDTO> gg = new List<MasterDataDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var responseTask1 = client.GetAsync("Employer/GetMasterDataByID/" + id);
                responseTask1.Wait();
                var result = responseTask1.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<MasterDataDTO>>();
                    readTask.Wait();
                    gg = readTask.Result.ToList();
                }
                else //web api sent error response 
                {
                    //log response status here..
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return PartialView("MasterData/_MasterDataInsert", gg != null && gg.Any() ? gg.FirstOrDefault() : new MasterDataDTO());

        }
        public ActionResult MasterRecordInsert(MasterDataType Type)
        {
            MasterDataDTO gg6 = null;
            if (TempData["masterData"] == null)
                gg6 = new MasterDataDTO { Type = Type, Isactive = true };
            else
                gg6 = (MasterDataDTO)TempData["masterData"];
            return PartialView("MasterData/_MasterDataInsert", gg6);
        }

        [HttpPost]
        public ActionResult MasterRecordInsert(MasterDataDTO model)
        {
            ModelState.Remove("Id");
            ModelState.Remove("Type");
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(value);
                    model.CreatedBy = ((UserSecurityToken)Session["CurrentUser"]).Id;
                    model.ModifiedBy = ((UserSecurityToken)Session["CurrentUser"]).Id;
                    var postTask = client.PostAsJsonAsync<MasterDataDTO>("Employer/InsertMasterdata", model);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["MaterDataType"] = model.Type;
                        return Json(new Response { Status = AjaxResponse.Success });
                    }

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return Json(new Response { Status = AjaxResponse.ModelError, Result = ModelUtil.RenderPartialToString("MasterData/_MasterDataInsert", model, ControllerContext) });
        }

        public ActionResult DeleteMasterData(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var postTask = client.DeleteAsync("DeleteMasterdata/" + id);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return Json(new Response { Status = AjaxResponse.Success }, JsonRequestBehavior.AllowGet);
                }

                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }
            return Json(new Response { Status = AjaxResponse.Failed }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PRODUCT

        public ActionResult Products()
        {
            return View("Product/Products");
        }

        public ActionResult CreateProduct()
        {
            return View("Product/Product");
        }

        public ActionResult EditProduct(long id)
        {
            return View("Product/Product");
        }

        public ActionResult SaveProduct()
        {
            return View("Product/Product");
        }

        public ActionResult DeleteProduct(long id)
        {
            return View("Product/Products");
        }

        #endregion
    }
}