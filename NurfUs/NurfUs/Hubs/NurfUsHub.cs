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
using NurfUs.Classes.Betting;

namespace NurfUs.Hubs
{
    public class NurfUsHub : Hub
    {
        private const int MILLISECONDS_PER_ROUND = 3000;

        private static DateTime LastChosen;
        internal static MatchDetail ChosenMatch;
        internal static ChampionListDto champs;
        private static List<NurfClient> nurfers = new List<NurfClient>();

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

        private static object lockObj;
        private static TimeSpan currentTimeSpan;
        private static Stopwatch stopWatch;
        private static Task timerTask;




        public void GetCurrentMatch()
        {
            Clients.Caller.newMatch(CreateGameDisplay(ChosenMatch));
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

                    nurfers.Add(newClient);
	            }
	        }

            Clients.Caller.userResponse(newClient);
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

            //Well we sorta just want like random bets to spawn.
            URFBetRound roundBet = URFBetRound.GenerateRandomBetRound(ChosenMatch);
            
        }

        internal static GameDisplay CreateGameDisplay(MatchDetail match)
        {
            GameDisplay gameDisplay = new GameDisplay()
            {
                SecondsElapsed = Convert.ToInt32(DateTime.Now.Subtract(LastChosen).TotalSeconds)*1000,
                MatchInterval = Convert.ToInt32(ConfigurationManager.AppSettings["NewMatchInterval"]),
                BlueTeam = new List<ParticipantDisplay>(),
                PurpleTeam = new List<ParticipantDisplay>(),
                BetType = BetType.Team,
                BetQuestion = "Which team won the game?"
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
    }
}