using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteamPlayerInvestigatorV2
{
    public sealed class SteamAPI
    {
        private static SteamAPI instance;
        private static readonly object padlock = new object();
        public List<Thread> threads = new List<Thread>();
        public string ApiKey = null, steamID = null;

        SteamAPI() { }
        public static SteamAPI Instance {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SteamAPI();
                    }
                    return instance;
                }
            }
        }

        public void startThread(string threadType, string xApiKey, string xSteamID) {

            ApiKey = xApiKey; steamID = xSteamID;
            if(threadType == "createPlayer") {
                Thread thread = new Thread(() =>
                {
                    Player tempPlayer = new Player();
                    tempPlayer = handleBans(returnApiReply(updateURI("bans")), tempPlayer);
                    if(tempPlayer.vacBanned || Suspect.Instance.playerData == null) {
                        //tempPlayer = handleSummary(returnApiReply(updateURI("summary")), tempPlayer);
                        //tempPlayer = handleLevel(returnApiReply(updateURI("level")), tempPlayer);
                        //tempPlayer = handleGameList(returnApiReply(updateURI("gameList")), tempPlayer);
                        //tempPlayer = handleRecentGameList(returnApiReply(updateURI("recentGames")), tempPlayer);
                        if (Suspect.Instance.playerData == null) {
                            Suspect.Instance.playerData = tempPlayer;
                            handleFriends(returnApiReply(updateURI("friends")));}
                        else { Suspect.Instance.suspectList.Add(tempPlayer); }
                    }//If player is banned or if player info is the suspect.
                });
                thread.Name = "createPlayer";
                threads.Add(thread);
                thread.Start();
            }//Creates a player object for player provided from steamID list and adds to suspectList if they have a vac ban, or assigns to suspect if data belongs to suspect
            else if (threadType == "friendsList") {
                Thread thread = new Thread(() =>
                {
                    handleFriends(returnApiReply(updateURI("friends")));
                });
                thread.Name = "friends";
                threads.Add(thread);
                thread.Start();
            }//Adds friends to steamID List
            
        }

        private string updateURI(string requestType)
        {
            string uri = null;
            switch (requestType)
            {
                case "summary":
                    uri = "https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + ApiKey + "&steamids=" + steamID;
                    break;
                case "friends":
                    uri = "https://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key=" + ApiKey + "&steamid=" + steamID + "&relationship=friend";
                    break;
                case "bans":
                    uri = "https://api.steampowered.com/ISteamUser/GetPlayerBans/v0001/?key=" + ApiKey + "&steamids=" + steamID;
                    break;
                case "level":

                    break;
                case "gameList":

                    break;
                case "recentGames":

                    break;
            }
            return uri;
        }

        private Player handleRecentGameList(string result, Player tempPlayer)
        {
            throw new NotImplementedException();
        }

        private Player handleGameList(string result, Player tempPlayer)
        {
            throw new NotImplementedException();
        }

        private Player handleLevel(string result, Player tempPlayer)
        {
            throw new NotImplementedException();
        }

        private Player handleBans(string result, Player tempPlayer)
        {
            if(result == "{}") { return tempPlayer; } //Private account
            #region Seperating Info from APIResponse
            result = result.Remove(0, 25);
            result = result.Remove((result.Length - 4), 4);
            result = result.Replace("\"", "");
            #endregion

            string[] splitResponse = result.Split(',');

            #region Extract Information
            for (int i = 0; i < splitResponse.Length; i++)
            {
                string[] tempArray = splitResponse[i].Split(':', 2);
                switch (tempArray[0])
                {
                    case "CommunityBanned":
                        tempPlayer.communityBanned = bool.Parse(tempArray[1]);
                        break;
                    case "VACBanned":
                        tempPlayer.vacBanned = bool.Parse(tempArray[1]);
                        break;
                    case "numberOfVACBans":
                        tempPlayer.numberOfVACBans = int.Parse(tempArray[1]);
                        break;
                    case "EconomyBan":
                        if (tempArray[1] != "none") { tempPlayer.economyBan = true; }
                        break;
                    case "DaysSinceLastBan":
                        tempPlayer.daysSinceLastBan = int.Parse(tempArray[1]);
                        break;
                }
            }
            #endregion

            return tempPlayer;
        }

        private void handleFriends(string result)
        {
            if (result == "{}") { return; } //If profile is private and friendsList cannot be accessed.
            #region Seperating Info from APIResponse
            result = result.Remove(0, 28);
            result = result.Remove((result.Length - 4), 4);
            result = result.Replace("\"", "");
            result = result.Replace(",{", "");
            #endregion
            Suspect suspect = Suspect.Instance;
            string[] splitResponse = result.Split('}');
            for (int i = 0; i < splitResponse.Length; i++)
            {
                string[] tempArray = splitResponse[i].Split(',', 2);
                tempArray = tempArray[0].Split(':');
                if (!suspect.steamIDList.Contains(tempArray[1]) && tempArray[1] != suspect.playerData.steamID) { suspect.steamIDList.Add(tempArray[1]); }
                 
                //If steamID is not in the current steamIDList and is not equal to the suspect themself.
            }

        }//Player doesnt matter in this one, as we aren't adding data to the player class but the suspect class instead.

        private Player handleSummary(string summary, Player tempPlayer)
        {
            #region Seperating Info from APIResponse
            summary = summary.Remove(0, 25);
            summary = summary.Remove((summary.Length - 4), 4);
            summary = summary.Replace("\"", "");
            #endregion

            string[] splitResponse = summary.Split(',');
            return tempPlayer;
;
        }

        public string returnApiReply(string uri)
        {
            HttpClient client;
            HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate };
            client = new HttpClient(handler);

            string reply = null;
            client.BaseAddress = new Uri(uri);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            reply = response.Content.ReadAsStringAsync().Result;
            return reply;
        }//Returns the reply of the result.
        internal bool forbiddenCheck(string APIKey, string steamID)
        {
            HttpClient client;
            HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate };
            client = new HttpClient(handler);

            string reply = null;
            client.BaseAddress = new Uri("https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + APIKey + "&steamids=" + steamID);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.ReasonPhrase == "Forbidden") { return true; }
            return false;
        }//Checks to see if provided APIKey is valid.
    }
}
