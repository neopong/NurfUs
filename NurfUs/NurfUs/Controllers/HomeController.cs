using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NurfUs.Classes;
using NurfUs.Models;

namespace NurfUs.Controllers
{
    public class HomeController : NurfUsController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(new SummonerSearch());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}