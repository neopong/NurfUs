using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NurfUs.Models.API;

namespace NurfUs.Classes.Betting.Questions
{
    public class QTeamMostTowerKills : IBetQuestion
    {
        public BetType BetType { get { return BetType.Team; } }

        public string BetQuestion { get { return "Which team got the most tower kills?"; } }

        public List<int> GetCorrectAnswerIds(MatchDetail match)
        {
            List<int> correctAnswers = new List<int>();

            if (match.Teams[0].TowerKills == match.Teams[1].TowerKills)
            {
                correctAnswers.Add(100);
                correctAnswers.Add(200);
            }
            else if (match.Teams[0].TowerKills > match.Teams[1].TowerKills)
            {
                correctAnswers.Add(100);
            }
            else
            {
                correctAnswers.Add(200);
            }

            return correctAnswers;
        }
    }
}