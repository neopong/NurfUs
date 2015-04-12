using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NurfUs.Models.API;

namespace NurfUs.Classes.Betting.Questions
{
    public class QFirstBlood : IBetQuestion
    {
        public BetType BetType { get { return BetType.Summoner; } }

        public string BetQuestion { get { return "Which summoner got First Blood?"; } }

        public int GetCorrectAnswerId(MatchDetail match)
        {
            int correctAnswer = -1;

            if (match.Timeline.Frames != null)
            {
                foreach (var frame in match.Timeline.Frames)
                {
                    if (frame.Events != null)
                    {
                        Event firstBloodEvent =
                            frame.Events.FirstOrDefault(e => e.EventType == "CHAMPION_KILL" && e.KillerId != 0);

                        if (firstBloodEvent != null)
                        {
                            correctAnswer = firstBloodEvent.KillerId;
                            break;
                        }
                    }
                }
            }

            return correctAnswer;
        }
    }
}