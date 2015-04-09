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

        [HttpPost]
        public async Task<ActionResult> Index(SummonerSearch search)
        {
            RESTResult<Dictionary<string, SummonerDto>> result =
                    await
                        RESTHelpers.RESTRequest<Dictionary<string, SummonerDto>>
                (
                    string.Format("{0}{1}/v1.4/summoner/by-name/{2}", BaseUrl.Replace("/na.", "/" + search.Region.ToLower() + "."), search.Region.ToLower(), search.Name),
                    "",
                    DevKey,
                    ""
                );

            if (result.Success)
            {
                search.Summoners = result.ReturnObject;
            }

            return View(search);
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