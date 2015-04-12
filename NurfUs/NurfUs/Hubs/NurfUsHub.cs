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
using NurfUs.Classes.Betting.Questions;


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
        internal const String EVENT_KEY_CHAMPION_KILL = "CHAMPION_KILL";
        private const int MILLISECONDS_PER_ROUND = 3000;

        private static DateTime LastChosen;
        internal static MatchDetail ChosenMatch;
        internal static IBetQuestion ChosenQuestion;
        internal static ChampionListDto champs;
        private static Dictionary<String, PlayerBet> PlayerBets;
        private static int CurrentCorrectAnswer;
        private static UserData userDataContext;
        private static List<NurfClient> nurfers = new List<NurfClient>();

        private static List<IBetQuestion> questions = new List<IBetQuestion>()
        {
            new QFirstBlood(),
            new QMostKills(),
            new QTeamWinner()
        };
        static NurfUsHub()
        {
            PlayerBets = new Dictionary<string, PlayerBet>();
            userDataContext = new UserData();
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
                    
                    UserInfo userInfo = new UserInfo();
                    userInfo.ASPNetUserId = newClient.Key;
                    userInfo.InCorrectGuesses = 0;
                    userInfo.CorrectGuesses = 0;
                    userInfo.TempUser = true;
                    userInfo.Currency = 5000;
                    userDataContext.UserInfoes.Add(userInfo);
                    userDataContext.SaveChangesAsync();
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
            //We've already pulled match info and stored the json on the local drive in the event the api is down.
            //If you don't have the matches stored on your local drive use the code below instead of the region below it
            /*
            Task<RESTResult<MatchDetail>> matchResult = RESTHelpers.RESTRequest<MatchDetail>
                (
                    "https://na.api.pvp.net/api/lol/na/v2.2/match/1778704162", 
                    "", 
                    "{apiKey}", 
                    "includeTimeline=true"
                );

            matchResult.Wait();
            
            if (matchResult.Result.Success)
            {
                ChosenMatch = matchResult.Result.ReturnObject;
            }
            else
            {
                //Do your error logic here
            }
            */

            #region Pull Match from local drive
            //***************************************************
            DirectoryInfo diMatchHistory = new DirectoryInfo(ConfigurationManager.AppSettings["MatchDirectory"]);
            int matchCount = diMatchHistory.GetFiles().Count();

            Random randomGameNum = new Random();
            string matchContent = File.ReadAllText(diMatchHistory.GetFiles()[randomGameNum.Next(matchCount)].FullName);

            JavaScriptSerializer jss = new JavaScriptSerializer();

            ChosenMatch = jss.Deserialize<MatchDetail>(matchContent);
            //***************************************************
            #endregion Pull Match from local drive

            LastChosen = DateTime.Now;

            ChosenQuestion = questions[new Random().Next(questions.Count)];
            CurrentCorrectAnswer = ChosenQuestion.GetCorrectAnswerId(ChosenMatch);

            PlayerBets.Clear();
        }

        internal static GameDisplay CreateGameDisplay(MatchDetail match)
        {
            GameDisplay gameDisplay = new GameDisplay()
            {
                SecondsElapsed = Convert.ToInt32(DateTime.Now.Subtract(LastChosen).TotalSeconds)*1000,
                MatchInterval = Convert.ToInt32(ConfigurationManager.AppSettings["NewMatchInterval"]),
                BlueTeam = new List<ParticipantDisplay>(),
                PurpleTeam = new List<ParticipantDisplay>(),
                BetType = ChosenQuestion.BetType,
                BetQuestion = ChosenQuestion.BetQuestion
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

        //Evaluates the results of the current match before starting a new one.
        internal static void EvaluateCurrentMatch()
        {
            //Only evaluate the results if there were any bets
            if (PlayerBets != null && PlayerBets.Count > 0)
            {
                foreach (var playerBetKvp in PlayerBets)
                {
                    var user = userDataContext.UserInfoes.Where(ud => ud.ASPNetUserId == playerBetKvp.Key).FirstOrDefault();
                    if (user != null)
                    {
                        if (playerBetKvp.Value.BetChoiceId == CurrentCorrectAnswer)
                        {
                            user.Currency += playerBetKvp.Value.BetAmount;
                            user.CorrectGuesses++;
                        }
                        else
                        {
                            user.Currency -= playerBetKvp.Value.BetAmount;
                            user.InCorrectGuesses++;
                        }
                    }
                }
                userDataContext.SaveChangesAsync();
            }
        }

        internal static async void UpdateUserInfo(UserInfo info)
        {
            
        }
    }
}