using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurfUs.Classes.Betting
{

    public class UserBet
    {
        int BetOptionIndex
        {
            get;
            set;
        }

        int BetAmount
        {
            get;
            set;
        }
    }
    public class URFBetRound : IBetRound
    {
        List<BetOption> BetOptions
        {
            get;
            set;
        }

        //UserId, with bit option Index
        Dictionary<String, int> UserBets
        {
            get;
            set;
        }

        public int WinningBetIndex
        {
            get;
            set;
        }

        public URFBetRound(List<BetOption> betOptions, int winningBetIndex)
        {
            BetOptions = betOptions;
            WinningBetIndex = winningBetIndex;
        }

        public void AddUserBet(String userId, int betOptionIndex)
        {
            if (UserBets.ContainsKey(userId))
            {
                UserBets[userId] = betOptionIndex;
            }
            else
            {
                UserBets.Add(userId, betOptionIndex);
            }
        }

    }
}