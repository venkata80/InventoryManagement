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
            if (Session["CurrentUser"] != null)
            {
                return View("Dashboard");
            }
            return RedirectToAction("UserLogin", "Account");
        }

        #region EMPLOYER

        public ActionResult Employers()
        {
            if (Session["CurrentUser"] != null)
            {
                return View("Employer/Employers");
            }
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult GetEmployers(bool ActiveFL = true)
        {
            if (Session["CurrentUser"] != null)
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
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult CreateEmployer()
        {
            if (Session["CurrentUser"] != null)
            {
                return View("Employer/CreateEmployer", new EmployerDTO() { Address = new AddressDTO { } });
            }
            return RedirectToAction("UserLogin", "Account");
        }
        [HttpPost]
        public ActionResult CreateEmployer(EmployerDTO student)
        {
            if (Session["CurrentUser"] != null)
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
                            return RedirectToAction("Employers");
                        }

                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
                return View("Employer/CreateEmployer", student);
            }
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult EditEmployer(Guid id)
        {
            if (Session["CurrentUser"] != null)
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
            return RedirectToAction("UserLogin", "Account");
        }

        [HttpPost]
        public ActionResult EditEmployer(EmployerDTO student)
        {
            if (Session["CurrentUser"] != null)
            {
                if (ModelState.IsValid)
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
                            return RedirectToAction("Employers");
                        }
                        else //web api sent error response 
                        {
                            //log response status here..

                            //  emplist =  IList.Empty<BaseEmployerDTO>();


                            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        }
                    }
                }
                return View("Employer/CreateEmployer", student);
            }
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult DeleteEmployer(Guid id)
        {
            if (Session["CurrentUser"] != null)
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
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult EnableEmployer(Guid id)
        {
            if (Session["CurrentUser"] != null)
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
            return RedirectToAction("UserLogin", "Account");
        }
        #endregion

        #region SUPPLIERS

        public ActionResult ViewSuppliers()
        {
            if (Session["CurrentUser"] != null)
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
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult CreateSupplier()
        {
            if (Session["CurrentUser"] != null)
            {
                SuppliersDTO model = new SuppliersDTO() { Isactive = true, Address = new AddressDTO() };
                return View("Supplier/CreateSupplier", model);
            }
            return RedirectToAction("UserLogin", "Account");
        }

        [HttpPost]
        public ActionResult CreateSupplier(SuppliersDTO student)
        {
            if (Session["CurrentUser"] != null)
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
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult EditSupplier(Guid id)
        {
            if (Session["CurrentUser"] != null)
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
            return RedirectToAction("UserLogin", "Account");
        }

        [HttpPost]
        public ActionResult EditSupplier(SuppliersDTO student)
        {
            if (Session["CurrentUser"] != null)
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

                        student.CreatedBy = ((UserSecurityToken)Session["CurrentUser"]).Id;
                        student.ModifiedBy = ((UserSecurityToken)Session["CurrentUser"]).Id;
                        client.BaseAddress = new Uri(value);
                        var postTask = client.PostAsJsonAsync<SuppliersDTO>("Employer/InsertSupplierData", student);
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
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult DeleteSuppliers(int id)
        {
            if (Session["CurrentUser"] != null)
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
            return RedirectToAction("UserLogin", "Account");
        }

        #endregion

        #region MASTER DATA

        public ActionResult MasterTab()
        {
            if (Session["CurrentUser"] != null)
            {
                return View("MasterData/MasterTab");
            }
            return RedirectToAction("UserLogin", "Account");
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult MasterDataView(MasterDataType type)
        {
            if (Session["CurrentUser"] != null)
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
            return RedirectToAction("UserLogin", "Account");
        }

        [HttpGet]
        public ActionResult MasterDataView1(int id)
        {
            if (Session["CurrentUser"] != null)
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
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult MasterRecordInsert(MasterDataType Type)
        {
            if (Session["CurrentUser"] != null)
            {
                MasterDataDTO gg6 = null;
                if (TempData["masterData"] == null)
                    gg6 = new MasterDataDTO { Type = Type, Isactive = true };
                else
                    gg6 = (MasterDataDTO)TempData["masterData"];
                return PartialView("MasterData/_MasterDataInsert", gg6);
            }
            return RedirectToAction("UserLogin", "Account");
        }

        [HttpPost]
        public ActionResult MasterRecordInsert(MasterDataDTO model)
        {
            if (Session["CurrentUser"] != null)
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
            return Json(new Response { Status = AjaxResponse.SessionExpired });
        }

        public ActionResult DeleteMasterData(int id)
        {
            if (Session["CurrentUser"] != null)
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
            return Json(new Response { Status = AjaxResponse.SessionExpired }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PRODUCT

        public ActionResult Products()
        {
            if (Session["CurrentUser"] != null)
            {
                return View("Product/Products");
            }
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult GetProducts(bool CoreItemFL = true)
        {
            if (Session["CurrentUser"] != null)
            {
                IList<EmployerDTO> emplist = null;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(value);
                    var responseTask = client.GetAsync("Employer/GetEmployers?CoreItemFL=" + CoreItemFL);
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
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult CreateProduct()
        {
            if (Session["CurrentUser"] != null)
            {
                return View("Product/Product");
            }
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult EditProduct(long id)
        {
            if (Session["CurrentUser"] != null)
            {
                return View("Product/Product");
            }
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult SaveProduct()
        {
            if (Session["CurrentUser"] != null)
            {
                return View("Product/Product");
            }
            return RedirectToAction("UserLogin", "Account");
        }

        public ActionResult DeleteProduct(long id)
        {
            if (Session["CurrentUser"] != null)
            {
                return View("Product/Products");
            }
            return RedirectToAction("UserLogin", "Account");
        }

        #endregion
    }
}