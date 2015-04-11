using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurfUs.Classes.Betting
{
    public class UserBet
    {
        public String UserId
        {
            get;
            set;
        }

        public int BetAmount
        {
            get;
            set;
        }

        public BetEntity ChoiceSuperiorEntity
        {
            get;
            set;
        }

        public BetEntity ChoiceInferiorEntity
        {
            get;
            set;
        }
    }
}