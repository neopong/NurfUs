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

        public List<int> GetCorrectAnswerIds(MatchDetail match)
        {
            List<int> correctAnswers = new List<int>();

            correctAnswers.Add(match.Participants.FirstOrDefault(p => p.Stats.FirstBloodKill).ParticipantId);

            return correctAnswers;
        }
    }
}