using NurfUs.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NurfUs.Classes.Betting;

namespace NurfUs.Classes.Betting.Questions
{
    public interface IBetQuestion
    {
        BetType BetType { get; }
        string BetQuestion { get; }
        List<int> GetCorrectAnswerIds(MatchDetail match);
    }
}