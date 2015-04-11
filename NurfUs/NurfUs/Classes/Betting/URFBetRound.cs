using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurfUs.Classes.Betting
{

  
    public class URFBetRound 
    {
        public BetEvent Event{
            get;
            set;
        }
        
        private Dictionary<String,UserBet> UserBets;

        public URFBetRound(BetEvent pEvent)
        {
            this.Event = pEvent;
            this.UserBets = new Dictionary<string, UserBet>();
        }

        public void AddUserBet(UserBet bet)
        {
            if (!UserBets.ContainsKey(bet.UserId))
            {
                UserBets.Add(bet.UserId, bet);
            }
            else
            {
                UserBets[bet.UserId] = bet;
            }
        }

        public void ChangeUserBetAmount(String userId, int newAmount)
        {
            if (UserBets.ContainsKey(userId))
            {
                UserBets[userId].BetAmount = newAmount;
            }
        }

        public void ChangeUserEntityChoice(String userId, BetEntity superiorEntity, BetEntity inferiorEntity)
        {
            if (UserBets.ContainsKey(userId))
            {
                UserBets[userId].ChoiceSuperiorEntity = superiorEntity;
                UserBets[userId].ChoiceInferiorEntity = inferiorEntity;
            }
        }

    }
}