using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NurfUs.Classes;
using System.Timers;
using NurfUs.Hubs;
using Microsoft.AspNet.SignalR;
using System.Web.Script.Serialization;
using NurfUs.Models.API;
using System.Configuration;

namespace NurfUs
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private Timer timer = new Timer();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            NurfUsController.DevKey = File.ReadAllText(HostingEnvironment.MapPath("~/APIKey.txt"));
            

            JavaScriptSerializer jss = new JavaScriptSerializer();

            string champData = File.ReadAllText(HostingEnvironment.MapPath("~/Cache/champion.json"));
            NurfUsHub.champs = jss.Deserialize<ChampionListDto>(champData);

            NurfUsHub.GenerateNewMatch();

            timer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["NewMatchInterval"]);
            timer.Elapsed += (o, t) =>
            {
                NurfUsHub.EvaluateCurrentMatch();
                NurfUsHub.GenerateNewMatch();
                GlobalHost.ConnectionManager.GetHubContext<NurfUsHub>().Clients.All.newMatch(NurfUsHub.CreateGameDisplay(NurfUsHub.ChosenMatch, NurfUsHub.ChosenBetType));
            };

            timer.Start();
        }
    }
}
