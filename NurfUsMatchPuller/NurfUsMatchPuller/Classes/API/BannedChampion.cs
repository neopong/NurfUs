using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurfUs.Models.API
{
    public class BannedChampion
    {
        public long ChampionId { get; set; }
        public int PickTurn { get; set; }
        public long TeamId { get; set; }
    }
}