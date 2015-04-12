using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NurfUs.Models.API;

namespace NurfUs.Classes.Betting.Questions
{
    public class QMostKills : IBetQuestion
    {
        public BetType BetType { get { return BetType.Summoner; } }

        public string BetQuestion { get { return "Which summoner got the most kills?"; } }

        public int GetCorrectAnswerId(MatchDetail match)
        {
            //Turns out they dont give us the stats object... so we gotta do this manually...
            int participantMostKills = 0;
            int currentHighKillCount = 0;

            foreach (var participant in match.Participants)
            {
                int curCount = 0;
                foreach (var frame in match.Timeline.Frames)
                {
                    if (frame.Events != null)
                    {
                        curCount += frame.Events.Where(e => e.EventType == NurfUs.Hubs.NurfUsHub.EVENT_KEY_CHAMPION_KILL && e.KillerId == participant.ParticipantId).Count();
                    }
                }
                if (curCount > currentHighKillCount)
                {
                    currentHighKillCount = curCount;
                    participantMostKills = participant.ParticipantId;
                }
            }

            return participantMostKills;
        }
    }
}