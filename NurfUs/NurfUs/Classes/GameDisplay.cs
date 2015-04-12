using NurfUs.Classes;
using NurfUs.Classes.Betting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurfUs.Classes
{
    public class GameDisplay
    {
        public int SecondsElapsed { get; set; }
        public int MatchInterval { get; set; }
        public List<ParticipantDisplay> BlueTeam { get; set; }
        public List<ParticipantDisplay> PurpleTeam { get; set; }
        public BetType BetType { get; set; }
        public string BetQuestion { get; set; }
    }
}