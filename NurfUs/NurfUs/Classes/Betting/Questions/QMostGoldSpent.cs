using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NurfUs.Models.API;

namespace NurfUs.Classes.Betting.Questions
{
    public class QMostGoldSpent : IBetQuestion
    {
        public BetType BetType { get { return BetType.Summoner; } }

        public string BetQuestion { get { return "Which summoner spent the most gold?"; } }

        public List<int> GetCorrectAnswerIds(MatchDetail match)
        {
            List<Participant> participantsWithMostDamage =
                match.Participants.Where(pmax => pmax.Stats.GoldSpent == match.Participants.Max(p => p.Stats.GoldSpent))
                    .ToList();

            return participantsWithMostDamage.Select(participant => participant.ParticipantId).ToList();
        }
    }
}