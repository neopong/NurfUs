using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using NurfUs.Models.API;
using NurfUs.Classes;
using System.Web.Hosting;
using System.IO;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Timers;
using System.Diagnostics;
using System.Threading.Tasks;
using NurfUs.Models;
using System.Data.Entity;


namespace NurfUs.Hubs
{
    internal class PlayerBet
    {
        public int BetAmount{
            get;
            set;
        }

        public String UserId{
            get;
            set;
        }

        public int BetChoiceId{
            get;
            set;
        }
    }

    public class NurfUsHub : Hub
    {
        public const int NEW_ACCOUNT_CURRENCY = 10000;

        private const int MILLISECONDS_PER_ROUND = 3000;

        private static DateTime LastChosen;
        internal static MatchDetail ChosenMatch;
        internal static BetType ChosenBetType;
        internal static ChampionListDto champs;
        private static Dictionary<String, PlayerBet> PlayerBets;

        private static List<NurfClient> nurfers = new List<NurfClient>();

        static NurfUsHub()
        {
            PlayerBets = new Dictionary<string, PlayerBet>();

        }

        public void Applause(string name, string key)
        {
            if (nurfers.FirstOrDefault(n => n.Name == name && n.Key == key) != null)
            {
                Clients.All.applause();
                Clients.All.broadcastMessage(name, name + " shares his love for URF's magestic spatula and fills the site with applause!");
            }
        }

        public void Fart(string name, string key)
        {
            if (nurfers.FirstOrDefault(n => n.Name == name && n.Key == key) != null)
            {
                Clients.All.fart();
                Clients.All.broadcastMessage(name, name + " has eaten too much feesh. Here comes the SPRAY!!!1one!");
            }
        }

        public void GetCurrentMatch()
        {
            int choice = new Random().Next(1);
            Clients.Caller.newMatch(CreateGameDisplay(ChosenMatch,ChosenBetType));
        }

        public void NewGuest(string guestName)
        {

            NurfClient newClient = new NurfClient();

            guestName = guestName.Trim();

            if (guestName.Length < 3)
            {
                newClient.Message = "Your name must be at least 3 characters long.";
            }
            else
	        {
                guestName = "Guest( " + guestName + " )";
                
                if (nurfers.FirstOrDefault(n => n.Name == guestName) != null)
                {
                    newClient.Message = "Guest name already taken. Try registering for a real account or try a new name";
                }
                else
	            {
                    newClient.Name = guestName;
                    newClient.Key = Guid.NewGuid().ToString();
                    newClient.Valid = true;

                    
	            }
	        }

            Clients.Caller.userResponse(newClient);
        }

        

        public void AddUserBet(String userId, int betId, int betAmount)
        {
            if (!PlayerBets.ContainsKey(userId))
            {
                PlayerBets.Add(userId, new PlayerBet() { UserId = userId });   
            }
            PlayerBets[userId].BetAmount = betAmount;
            PlayerBets[userId].BetChoiceId = betId;

            //You can call a client function to subtract their visible amount of currenty or something
            //After this function.:

            //client.DoSomethingCool();
        }

        public bool Send(string name, string key, string message)
        {
            if (nurfers.FirstOrDefault(n => n.Name == name && n.Key == key) != null)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    //Call the broadcastMessage method to update clients.
                    Clients.All.broadcastMessage(name, message);
                }
                
                return true;
            }

            return false;
        }

        //this is where we will add the new event
        internal static void GenerateNewMatch()
        {
            

            DirectoryInfo diMatchHistory = new DirectoryInfo(ConfigurationManager.AppSettings["MatchDirectory"]);
            int matchCount = diMatchHistory.GetFiles().Count();

            Random randomGameNum = new Random();
            string matchContent = File.ReadAllText(diMatchHistory.GetFiles()[randomGameNum.Next(matchCount)].FullName);

            //We should keep a cache of champions in memory
            JavaScriptSerializer jss = new JavaScriptSerializer();

            ChosenMatch = jss.Deserialize<MatchDetail>(matchContent);
            LastChosen = DateTime.Now;
            
            //I am having issues getting the participant id from the participant object
            //So I am going to do a simple assignment for now
            for (int i = 0; i < ChosenMatch.ParticipantIdentities.Count(); i++)
            {
                if (ChosenMatch.Participants[i].ParticipantId == 0)
                {
                    ChosenMatch.Participants[i].ParticipantId = ChosenMatch.ParticipantIdentities[i].ParticipantId;
                }
            }

            //Well we sorta just want like random bets to spawn.
            int choice = new Random().Next(3);
            switch (choice)
            {
                case 0:
                    ChosenBetType = BetType.SummonerMostKills;
                    break;
                case 1:
                    ChosenBetType = BetType.TeamWinner;
                    break;
                case 2:
                    ChosenBetType = BetType.SummonerFirstBlood;
                    break;
            }

            PlayerBets.Clear();
        }

        internal static GameDisplay CreateGameDisplay(MatchDetail match, BetType betType)
        {
            GameDisplay gameDisplay = new GameDisplay()
            {
                SecondsElapsed = Convert.ToInt32(DateTime.Now.Subtract(LastChosen).TotalSeconds)*1000,
                MatchInterval = Convert.ToInt32(ConfigurationManager.AppSettings["NewMatchInterval"]),
                BlueTeam = new List<ParticipantDisplay>(),
                PurpleTeam = new List<ParticipantDisplay>(),
                BetType = betType,
                BetQuestion = GetRoundMessage(betType)
            };
            
            foreach (Participant participant in ChosenMatch.Participants.Where(p => p.TeamId == 100))
            {
                gameDisplay.BlueTeam.Add(
                    new ParticipantDisplay() 
                    { 
                        ParticipantId = participant.ParticipantId,
                        ChampionImage = champs.Data.Values.FirstOrDefault(c => c.Id == participant.ChampionId).Image.Full
                    });
            }

            foreach (Participant participant in ChosenMatch.Participants.Where(p => p.TeamId == 200))
            {
                gameDisplay.PurpleTeam.Add(
                    new ParticipantDisplay()
                    {
                        ParticipantId = participant.ParticipantId,
                        ChampionImage = champs.Data.Values.FirstOrDefault(c => c.Id == participant.ChampionId).Image.Full
                    });
            }

            return gameDisplay;
        }

        private static String GetRoundMessage(BetType betType)
        {
            switch (betType)
            {
                case BetType.SummonerMostKills:
                    return "Which summoner got the most kills?";
                case BetType.SummonerFirstBlood:
                    return "Which summoner got First Blood?";
                case BetType.TeamWinner :
                    return "Which team won the game?";
                default:
                    break;
            }
            return "";
        }
    }
}