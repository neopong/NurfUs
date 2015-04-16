using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NurfUs.Models.API;

namespace NurfUs.Classes.Betting.Questions
{
    public class QTeamFirstInhibitor : IBetQuestion
    {
        public BetType BetType { get { return BetType.Team; } }

        public string BetQuestion { get { return "Which team destroyed the first inhibitor?"; } }

        public List<int> GetCorrectAnswerIds(MatchDetail match)
        {
            List<int> correctAnswers = new List<int>();
            correctAnswers.Add(match.Teams.Where(t => t.FirstInhibitor).FirstOrDefault().TeamId);

            return correctAnswers;
        }
    }
}