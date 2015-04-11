using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NurfUs.Classes.Betting
{
    public interface IBetRound
    {
        List<BetOption> GetBetOptions();

        void AddUserBet(String userId, int betOptionIndex);
    }
}
