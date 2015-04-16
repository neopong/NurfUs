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

        [Route("LeaderBoard")]
        public ActionResult LeaderBoard()
        {
            UserData userDataContext = new UserData();

            return View(userDataContext.UserInfoes.OrderByDescending(u => u.Currency));
        }
    }
}