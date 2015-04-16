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

        public List<int> GetCorrectAnswerIds(MatchDetail match)
        {
            List<Participant> participantsWithMostKills =
                match.Participants.Where(pmax => pmax.Stats.Kills == match.Participants.Max(p => p.Stats.Kills))
                    .ToList();

            return participantsWithMostKills.Select(participant => participant.ParticipantId).ToList();
        }
    }
}