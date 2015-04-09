using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace NurfUs.Classes
{
    public class NurfUsController : Controller
    {
        public static string DevKey = "";
        public static string GoogleAnalyticsKey = ConfigurationManager.AppSettings["GoogleAnalyticsKey"];

        public string BaseUrl
        {
            get { return ConfigurationManager.AppSettings["BaseUrl"]; }
        }
    }
}