using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurfUs.Models
{
    public class SummonerDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public int profileIconId { get; set; }
        public int summonerLevel { get; set; }
        public long revisionDate { get; set; }
        public bool InGame { get; set; }
    }
}