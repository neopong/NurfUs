using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurfUs.Classes.Betting
{
    //Pass a JSON match object and it will generate a Bet Event
    public class BetEventFactory
    {
        private static ulong uniqueEntityId = 0;

        internal static BetEvent GeneratePvPKillEvent(Models.API.MatchDetail matchDetail)
        {
            throw new NotImplementedException();
        }

        internal static BetEvent GeneratePvTKillEvent(Models.API.MatchDetail matchDetail)
        {
            throw new NotImplementedException();
        }

        internal static BetEvent GenerateFirstBloodKillEvent(Models.API.MatchDetail matchDetail)
        {
            throw new NotImplementedException();
        }
    }
}