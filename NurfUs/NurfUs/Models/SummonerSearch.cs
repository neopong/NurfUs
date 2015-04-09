using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NurfUs.Models.API;

namespace NurfUs.Models
{
    public class SummonerSearch
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public Dictionary<string, SummonerDto> Summoners { get; set; }
        public ChampionListDto Champions { get; set; }
    }
}