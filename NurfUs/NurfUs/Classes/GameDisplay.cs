﻿using System;
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
    }
}