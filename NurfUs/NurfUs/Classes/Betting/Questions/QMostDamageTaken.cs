using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NurfUs.Models.API;

namespace NurfUs.Classes.Betting.Questions
{
    public class QMostDamageTaken : IBetQuestion
    {
        public BetType BetType { get { return BetType.Summoner; } }

        public string BetQuestion { get { return "Which summoner took the most damage?"; } }

        public List<int> GetCorrectAnswerIds(MatchDetail match)
        {
            List<Participant> participantsWithMostDamage =
                match.Participants.Where(pmax => pmax.Stats.TotalDamageTaken == match.Participants.Max(p => p.Stats.TotalDamageTaken))
                    .ToList();

            return participantsWithMostDamage.Select(participant => participant.ParticipantId).ToList();
        }
    }
}