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

        public String ConnectionId
        {
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
        private static List<int> CurrentCorrectAnswers;

        internal static Dictionary<String, NurfClient> Nurfers = new Dictionary<String, NurfClient>();

        private static List<IBetQuestion> questions = new List<IBetQuestion>()
        {
            new QFirstBlood(),
            new QMostKills(),
            new QTeamWinner(),
            new QTeamMostTowerKills()
        };

        static NurfUsHub()
        {
            PlayerBets = new Dictionary<string, PlayerBet>();
        }

        public override Task OnConnected()
        {
            Task t = base.OnConnected();
            AddNurferConnection();
            return t;
        }


        public void Applause(string name, string key)
        {
            int applauseCost = 40000;

            var nurfClient = Nurfers.FirstOrDefault(n => n.Value.Name == name && n.Value.Key == key).Value;
            if (nurfClient != null && nurfClient.UserInfo.Currency >= applauseCost)
            {
                if (SubtractMoney(key, applauseCost))
                {
                    nurfClient.UserInfo.Currency -= applauseCost;
                    Clients.All.applause();
                    Clients.All.broadcastMessage("System", name + " shares his love for URF's magestic spatula and fills the site with applause!");
                }
            }
        }

        public void Fart(string name, string key)
        {
            //Abstract to somewhere later
            int fartCost = 30000;
            var nurfClient = Nurfers.FirstOrDefault(n => n.Value.Name == name && n.Value.Key == key).Value;
            if (nurfClient != null && nurfClient.UserInfo.Currency >= fartCost)
            {
                if(SubtractMoney(nurfClient.UserInfo.ASPNetUserId,fartCost))
                {
                    nurfClient.UserInfo.Currency -= fartCost;
                    Clients.All.fart();
                    Clients.All.broadcastMessage("System", name + " has eaten too much feesh. Here comes the SPRAY!!!1one!");
                }
            }
            else
            {
                //Display something
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
                UserData userDataContext = new UserData();
                guestName = "Guest( " + guestName + " )";
                if 
                (
                    Nurfers.FirstOrDefault(n => n.Value.Name.ToLower() == guestName.ToLower()).Value != null 
                    || 
                    userDataContext.UserInfoes.FirstOrDefault(u => u.UserKey == guestName) != null
                )
                {
                    newClient.Message = "Guest name already taken. Try registering for a real account or try a new name";
                }
                else
	            {
                    newClient.Name = guestName;
                    newClient.Key = Guid.NewGuid().ToString();
                    newClient.Valid = true;
                    UserData dataContext = new UserData();
                    UserInfo userInfo = new UserInfo();
                    userInfo.ASPNetUserId = newClient.Key;
                    userInfo.InCorrectGuesses = 0;
                    userInfo.CorrectGuesses = 0;
                    userInfo.TempUser = true;
                    userInfo.Currency = 5000;
                    userInfo.UserKey = guestName;
                    dataContext.UserInfoes.Add(userInfo);
                    dataContext.SaveChangesAsync();
                    newClient.UserInfo = userInfo;
                    newClient.SignalRConnectionId = Context.ConnectionId;
                    Nurfers.Add(newClient.Key, newClient);
	            }
	        }
            
            Clients.Caller.userResponse(newClient);
        }

        public bool AddNurferConnection()
        {
            var clientKey = "";
            var clientName = "";
            
            if (Context.Request.Cookies.ContainsKey("clientKey"))
                clientKey = Context.Request.Cookies["clientKey"].Value;

            if (Context.Request.Cookies.ContainsKey("clientName"))
                clientName = Context.Request.Cookies["clientName"].Value;

            String connId = Context.ConnectionId;

            if (!String.IsNullOrEmpty(clientKey) 
                && !String.IsNullOrEmpty(clientName) 
                && !String.IsNullOrEmpty(connId))
            {
                UserData dataContext = new UserData();
                NurfClient newClient = new NurfClient();
                newClient.Name = clientName;
                newClient.Key = clientKey;
                newClient.Valid = true;

                var userInfo = dataContext.UserInfoes.Where(u => u.ASPNetUserId == clientKey).FirstOrDefault();
                if (!Nurfers.ContainsKey(clientKey))
                {
                    if (userInfo != null)
                    {
                        newClient.UserInfo = userInfo;
                        newClient.SignalRConnectionId = connId;
                        Nurfers.Add(clientKey, newClient);
                    }
                }
                else
                {
                    Nurfers[clientKey].Name = clientName;
                    Nurfers[clientKey].SignalRConnectionId = connId;

                }
                if (Nurfers.ContainsKey(clientKey))
                {
                    Clients.Client(Nurfers[clientKey].SignalRConnectionId).displayCurrency(Nurfers[clientKey].UserInfo.Currency);
                    return true;
                }
            }
            return false;
        }

        //30,000 - 50,000
        public bool SubtractMoney(String userId, int money)
        {

            UserData dataContext = new UserData();
            
            var query = dataContext.UserInfoes.Where(c => c.ASPNetUserId == userId).FirstOrDefault();

            if (query != null)
            {
                query.Currency -= money;
                dataContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool SubtractMoney(NurfClient client, int money)
        {
            bool success = SubtractMoney(client.Key, money);
            if(success){
                client.UserInfo.Currency -= money;
            }
            return success;
        }

        public NurfClient GetClientFromDatabase(String userId)
        {
            
            UserData dataContext = new UserData();

            var userInfo = dataContext.UserInfoes.Where(c => c.ASPNetUserId == userId).FirstOrDefault();

            if (userInfo != null)
            {
                NurfClient client = new NurfClient();
                client.Key = userInfo.ASPNetUserId;
                client.Name = userInfo.UserKey;
                client.Valid = true;
                client.UserInfo = userInfo;

                return client;
            }
            return null;
        }

        public bool AddUserBet(String userId, int betAmount, int betId)
        {
            UserData userDataContext = new UserData();
            var user = userDataContext.UserInfoes.FirstOrDefault(cl => cl.ASPNetUserId == userId);
            bool success = false;
            if (user != null)
            {
                success = true;
                if (betAmount > user.Currency || betAmount <= 0)
                {
                    success = false;
                }
                else
                {
                    if (!PlayerBets.ContainsKey(userId))
                    {
                        PlayerBets.Add(userId, new PlayerBet() { UserId = userId });
                    }
                    PlayerBets[userId].BetAmount = betAmount;
                    PlayerBets[userId].BetChoiceId = betId;
                }
            }
            return success;
        }
        public bool RemoveUserBet(String userId)
        {
            if (PlayerBets.ContainsKey(userId))
            {
                PlayerBets.Remove(userId);
                return true;
            }
            return false;
        }

        public bool Send(string name, string key, string message)
        {
            if (Nurfers.FirstOrDefault(n => n.Value.Name == name && n.Value.Key == key).Value != null)
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
            CurrentCorrectAnswers = ChosenQuestion.GetCorrectAnswerIds(ChosenMatch);
#if DEBUG
            Debug.WriteLine(string.Join(",", CurrentCorrectAnswers));
#endif

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
                UserData userDataContext = new UserData();
                foreach (var playerBetKvp in PlayerBets)
                {
                    String curUserId = playerBetKvp.Key;
                    var user = userDataContext.UserInfoes.Where(ud => ud.ASPNetUserId == playerBetKvp.Key).FirstOrDefault();
                    if (user != null)
                    {
                        if (CurrentCorrectAnswers.Contains(playerBetKvp.Value.BetChoiceId))
                        {
                            user.Currency += playerBetKvp.Value.BetAmount;
                            user.CorrectGuesses++;
                        }
                        else
                        {
                            user.Currency -= playerBetKvp.Value.BetAmount;
                            user.InCorrectGuesses++;
                        }

                        if (Nurfers.ContainsKey(curUserId))
                        {
                            GlobalHost
                                .ConnectionManager
                                .GetHubContext<NurfUsHub>()
                                .Clients
                                .Client(Nurfers[curUserId].SignalRConnectionId)
                                .displayCurrency(user.Currency);

                            Nurfers[curUserId].UserInfo = user;
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