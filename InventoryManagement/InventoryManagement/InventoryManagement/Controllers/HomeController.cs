using InventoryManagement.Models;
using InventoryManagement.Util;
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
       
    }
}
