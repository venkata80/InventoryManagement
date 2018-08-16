using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace InventoryManagement.Controllers
{
    public class HomeController : Controller
    {
        string value = System.Configuration.ConfigurationManager.AppSettings["urlname"];
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        #region Employer

        public ActionResult ViewEmployer()
        {
            ViewBag.Title = "Home Page";
            IList<BaseEmployerDTO> emplist = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var responseTask = client.GetAsync("GetEmployee");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<BaseEmployerDTO>>();
                    readTask.Wait();

                    emplist = readTask.Result;
                }
                else //web api sent error response 
                {
                    emplist = new List<BaseEmployerDTO>();
                    //log response status here..

                    //  emplist =  IList.Empty<BaseEmployerDTO>();


                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return View(emplist);
        }

        public ActionResult CreateEmployer()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateEmployer(BaseEmployerDTO student)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var postTask = client.PostAsJsonAsync<BaseEmployerDTO>("InsertEmployerData", student);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("ViewEmployer");
                }

                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }
            //List<BaseEmployerDTO> emplist = new List<BaseEmployerDTO>();

            return View(student);
        }

        public ActionResult EditEmployer(int id)
        {
            BaseEmployerDTO emplist = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var responseTask = client.GetAsync("GetEmployeeById/" + id);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<BaseEmployerDTO>();
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
            return View(emplist);
        }
        [HttpPost]
        public ActionResult EditEmployer(BaseEmployerDTO student)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var postTask = client.PutAsJsonAsync("UpdateEmployerData", student);
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
            return View();
        }

        public ActionResult DeleteEmployer(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var deleteTask = client.DeleteAsync("DeleteEmployerData/" + id);
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
            return View();
        }
        #endregion

        public ActionResult ViewSuppliers()
        {
            ViewBag.Title = "Home Page";
            IList<SuppliersDTO> emplist = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var responseTask = client.GetAsync("GetAllSuppliers");
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

            return View(emplist);
        }

        public ActionResult CreateSupplier()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateSupplier(SuppliersDTO student)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var postTask = client.PostAsJsonAsync<SuppliersDTO>("InsertSupplierData", student);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("ViewSuppliers");
                }

                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }
            //List<BaseEmployerDTO> emplist = new List<BaseEmployerDTO>();

            return View(student);
        }

        public ActionResult EditSupplier(int id)
        {
            SuppliersDTO emplist = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var responseTask = client.GetAsync("GetSupplierById/" + id);
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
            return View(emplist);
        }
        [HttpPost]
        public ActionResult EditSupplier(SuppliersDTO student)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var postTask = client.PutAsJsonAsync("UpdateSupplierData", student);
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
            return View();
        }

        public ActionResult DeleteSuppliers(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var deleteTask = client.DeleteAsync("DeleteSupplierData/" + id);
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

        public ActionResult MasterTab()
        {
            return View();
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
                var responseTask1 = client.GetAsync("GetMasterDataBy");
                responseTask1.Wait();
                var result = responseTask1.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<MasterDataDTO>>();
                    readTask.Wait();

                    masterlist = readTask.Result.Where(s=>s.Type== type).ToList();
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
            return PartialView("_MasterDataList", masterlist);
        }
        MasterDataDTO tt = null;

        [HttpGet]
        public ActionResult MasterDataView1(int id)
        {
             tt = new MasterDataDTO();
           List <MasterDataDTO> gg = new List<MasterDataDTO>();
            gg = (List<MasterDataDTO>)TempData["masterDataList"];
            if (gg !=null && gg.Count>0)
            {
                 tt= gg.Where(d => d.Id == id).SingleOrDefault();
                TempData["masterData"] = tt;

            }
            return RedirectToAction("MasterTab");

        }
        public ActionResult MasterRecordInsert(MasterDataType Type)
        {
            MasterDataDTO gg6 = null;
            if (TempData["masterData"] == null)
                gg6 = new MasterDataDTO { Type = Type,Isactive=true };
            else
                gg6 = (MasterDataDTO)TempData["masterData"];
            return PartialView("_MasterDataInsert", gg6);
        }

        [HttpPost]
        public ActionResult MasterRecordInsert(MasterDataDTO model)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Student Name already exists.");
                return RedirectToAction("MasterTab");

            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(value);
                var postTask = client.PostAsJsonAsync<MasterDataDTO>("InsertMasterdata", model);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["MaterDataType"] = model.Type;
                    return RedirectToAction("MasterTab");
                }

                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }
            //List<BaseEmployerDTO> emplist = new List<BaseEmployerDTO>();

            //return View(student);
            //MasterDataDTO gg6 = new MasterDataDTO();
            //return PartialView("_MasterDataInsert", gg6);
            // return PartialViewResult("_InsertMasterData", gg6);
           // model = new MasterDataDTO();
            return View(model);

        }
    }
}
