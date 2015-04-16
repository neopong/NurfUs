using NurfUs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace NurfUs.Classes
{
    public class NurfClient
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public bool Valid { get; set; }
        public string Message { get; set; }

        [JsonIgnore]
        public String SignalRConnectionId { get; set; }
        [JsonIgnore]
        public UserInfo UserInfo { get; set; }
    }
}