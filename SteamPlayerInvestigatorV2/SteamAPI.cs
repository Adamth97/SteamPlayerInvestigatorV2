using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteamPlayerInvestigatorV2
{
    public class SteamAPI : SteamPlayerInvestigator
    {
        public List<Thread> threads = new List<Thread>();
        public string ApiKey = null, suspectID = null;

        public SteamAPI(string APIKey, string SteamID) { ApiKey = APIKey; suspectID = SteamID; }
        public void assignToSuspectData() { 
            handleSummary(returnApiReply(updateURI("summary", suspectID)), suspectID); //Gets suspects Summary Data 
            //handleLevel(returnApiReply(updateURI("level", suspectID)), suspectID);//Gets suspects level
            handleGameList(returnApiReply(updateURI("gameList", suspectID)), suspectID);//Gets suspects gameList
            //handleRecentGameList(returnApiReply(updateURI("recentGames", suspectID)), suspectID); // Gets suspects RecentGames
            handleFriends(returnApiReply(updateURI("friends", suspectID)), suspectID); //Gets users friends.
        }//Does all queries on suspect and assigns the data
        public void getFriendsofFriends(List<string> steamList) {
            int suspectFriendCount = steamList.Count;
            for (int i = 0; i < suspectFriendCount; i++)
            {
                string steamID = steamList[i];
                Thread thread = new Thread(() =>
                {
                    handleFriends(returnApiReply(updateURI("friends", steamID)), steamID);
                });
                thread.Name = "friends";
                threads.Add(thread); thread.Start();
            }
            waitForAllThreads();
        }//Does all friends queries, gets friends of friends of the suspect and adds them to the steamIDList
        public void startSpecificThreads(string steamIDs, string threadType)
        {
            Thread thread = new Thread(() =>
                {
                    if (threadType == "Bans") { handleBans(returnApiReply(updateURI("bans", steamIDs)), steamIDs); }
                    else if (threadType == "Summaries") { handleSummary(returnApiReply(updateURI("summary", steamIDs)), steamIDs); }
                });
            threads.Add(thread); thread.Start();
        }//Starts the banThread to sendRequest and handle response.
        public void gatherSuspects()
        {
            string steamIDs = "";
            for (int i = 0; i < Suspect.Instance.steamIDList.Count; i++)
            {
                if (threads.Count == 50) { waitForAllThreads(); } //Frees up threads
                if (i % 100 == 0 && i != 0) { 
                    startSpecificThreads(steamIDs.Substring(0, steamIDs.Length - 1), "Bans"); steamIDs = ""; }//If its been 100 people.
                steamIDs += Suspect.Instance.steamIDList[i] + ",";
            }
            startSpecificThreads(steamIDs, "Bans");
            waitForAllThreads();
        }
        public void gatherSummaries() {
            string steamIDs = "";
            if (Suspect.Instance.playerData == null) { steamIDs = suspectID; }
            else {
                for (int i = 0; i < Suspect.Instance.suspectList.Count; i++)
                {
                    if (threads.Count == 50) { waitForAllThreads(); } //Frees up threads
                    if (i % 100 == 0 && i != 0) { startSpecificThreads(steamIDs.Substring(0, steamIDs.Length - 1), "Summaries"); steamIDs = ""; }//If its been 100 people.
                    steamIDs += Suspect.Instance.suspectList[i].steamID + ",";
                }
            }
            startSpecificThreads(steamIDs, "Summaries");
            waitForAllThreads();
        }
        public void gatherBPGameList() {
            foreach (Player player in Suspect.Instance.suspectList) { 
                if(player.communityVisibilityState == 3) {
                    Thread thread = new Thread(() =>
                    {
                        handleGameList(returnApiReply(updateURI("gameList", player.steamID)), player.steamID);
                    });
                    threads.Add(thread); thread.Start();
                }
            }
            waitForAllThreads();
        }
        public void gatherBPRecentGames() { }
        public void gatherBPLevel() { }
        private void waitForAllThreads()
        {
            while (threads.Count != 0)
            {
                for (int i = 0; i < threads.Count; i++)
                {
                    if (threads[i].ThreadState == ThreadState.Stopped) { threads.RemoveAt(i); i--; }
                }
            }
        }//Waits for all threads to be done.
        private string updateURI(string requestType, string steamID)
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
                    uri = "https://api.steampowered.com/IPlayerService/GetSteamLevel/v1/?key=" + ApiKey + "&steamid=" + steamID;
                    break;
                case "gameList":
                    uri = "https://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + ApiKey + "&steamid=" + steamID;
                    break;
                case "recentGames":
                    uri = "https://api.steampowered.com/IPlayerService/GetRecentlyPlayedGames/v0001/?key=" + ApiKey + "&steamid=" + steamID;
                    break;
            }
            return uri;
        }
        private Player handleRecentGameList(string result, Player tempPlayer)
        {
            throw new NotImplementedException();
        }
        private void handleGameList(string result, string steamID)
        {
            if (result.Contains("429 Too Many Requests"))
            {
                Thread.Sleep(3000);
                handleGameList(returnApiReply(updateURI("gameList", steamID)), steamID);
            }//If too many requests, waits a second and then redoes the request.

            else if (result != "{\"response\":{}}")
            {
                #region Seperating Data from JSON
                result = result.Substring(38, result.Length - 44);
                result = result.Replace("\"", "").Replace("{", "");
                string[] gameList = result.Split("},");
                #endregion

                #region Assigning data to player class from JSON 
                foreach (string game in gameList)
                {
                    string[] gameData = game.Split(','); //Splits game from gameList into sections
                    Game newGame = new Game();
                    for (int i = 0; i < gameData.Length; i++)
                    {
                        string[] tempArray = gameData[i].Split(':', 2);
                        switch (tempArray[0])
                        {
                            case "appid":
                                newGame.gameID = (int.Parse(tempArray[1]));
                                break;
                            case "playtime_forever":
                                newGame.playTime = (int.Parse(tempArray[1]));
                                break;
                            case "rtime_last_played":
                                try { newGame.lastPlayed = (int.Parse(tempArray[1]));} catch { }
                                
                                break;
                        }
                    }//Assigns data to relevant variables in game class.

                    if (steamID == suspectID) { Suspect.Instance.playerData.gameList.Add(newGame); } //Adds to suspect
                    else { Suspect.Instance.suspectList.Find(i => i.steamID == steamID).gameList.Add(newGame); } //Adds to bannedPlayer
                }
                #endregion
            }
        }
        private Player handleLevel(string result, Player tempPlayer)
        {
            if (result != "{\"response\":{}}" && !result.Contains("Access Denied"))
            {
                string reply = result.Substring(14, result.Length - 16);
                string[] splitReply = reply.Split(':');
                tempPlayer.steamLevel = int.Parse(splitReply[1]);
            }
            return tempPlayer;
        }//Removes playerLevel from result and assigns it to the player.
        private void handleBans(string result,string steamIDs) {
            if (result.Contains("429 Too Many Requests"))
                    {
                        Thread.Sleep(3000);
                        handleBans(returnApiReply(updateURI("bans", steamIDs)), steamIDs);
                    }//If too many requests, waits a second and then redoes the request.
            else if (result != "{}") {

                #region Seperating Info from APIResponse
                result = result.Remove(0, 13);
                result = result.Remove((result.Length - 4), 4);
                result = result.Replace("\"", "");
                #endregion

                List<string> splitResponse = result.Split("},{").ToList(); //Seperates them into Players.

                for (int i = 0; i < splitResponse.Count; i++) { 
                    if (splitResponse[i].Contains("VACBanned:false")) { 
                        splitResponse.RemoveAt(i); i--; } } //Removes all players that arent banned out of the 100 players. (100 max, may not be 100) }
                
                for (int i = 0; i < splitResponse.Count; i++)
                {
                    Player bannedPlayerData = new Player();

                    string[] bannedPlayerInfo = splitResponse[i].Split(',');

                    for (int j = 0; j < bannedPlayerInfo.Length; j++) {

                        string[] tempArray = bannedPlayerInfo[j].Split(':', 2);

                        switch (tempArray[0])
                        {
                            case "SteamId":
                                bannedPlayerData.steamID = tempArray[1];
                                break;
                            case "VACBanned":
                                bannedPlayerData.vacBanned = bool.Parse(tempArray[1]);
                                break;
                            case "NumberOfVACBans":
                                bannedPlayerData.numberOfVACBans = int.Parse(tempArray[1]);
                                break;
                            case "DaysSinceLastBan":
                                bannedPlayerData.daysSinceLastBan = int.Parse(tempArray[1]);
                                break;
                        }
                    }
                    Suspect.Instance.suspectList.Add(bannedPlayerData);
                }
            }
        }
        private void handleFriends(string result, string steamID)
        {
            if (result == "{}" || result.Contains("Access Denied")) { return; } //If profile is private and friendsList cannot be accessed.

            if (result.Contains("429 Too Many Requests"))
            {
                Thread.Sleep(3000);
                handleFriends(returnApiReply(updateURI("bans", steamID)), steamID);
            }//If too many requests, waits a second and then redoes the request.
            else
            {
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
                    if (tempArray[1] != suspectID) { suspect.steamIDList.Add(tempArray[1]); }

                    //If steamID is not in the current steamIDList and is not equal to the suspect themself.
                }
            }

        }//Player doesnt matter in this one, as we aren't adding data to the player class but the suspect class instead.
        private void handleSummary(string result, string steamIDs)
        {
            if (result.Contains("429 Too Many Requests"))
            {
                Thread.Sleep(3000);
                handleSummary(returnApiReply(updateURI("summary", steamIDs)), steamIDs);
            }//If too many requests, waits a second and then redoes the request.
            else if (result != "{}")
            {

                #region Seperating Info from APIResponse
                result = result.Remove(0, 25);
                result = result.Remove((result.Length - 4), 4);
                result = result.Replace("\"", "");
                #endregion
                List<string> splitResponse = result.Split("},{").ToList(); //Seperates them into Players.

                for (int i = 0; i < splitResponse.Count; i++)
                {
                    string[] bannedPlayerInfo = splitResponse[i].Split(',');
                    string[] tempArray = bannedPlayerInfo[0].Split(','); 
                    tempArray = tempArray[0].Split(':');

                    Player tempPlayer = new Player();
                    if (Suspect.Instance.playerData != null) { 
                        tempPlayer = Suspect.Instance.suspectList.Find(i => i.steamID == tempArray[1]); }
                    else { tempPlayer.steamID = suspectID; }

                    for (int j = 1; j < bannedPlayerInfo.Length; j++)
                    {
                        tempArray = bannedPlayerInfo[j].Split(':');

                        switch (tempArray[0])
                        {
                            case "communityvisibilitystate":
                                tempPlayer.communityVisibilityState = int.Parse(tempArray[1]);
                                break;
                            case "personaname":
                                tempPlayer.personaName = tempArray[1];
                                break;
                            case "timecreated":
                                tempPlayer.timeCreated = int.Parse(tempArray[1]);
                                break;
                            case "avatar":
                                tempPlayer.avatar = "https:" + tempArray[2];
                                break;
                            case "primaryclanid":
                                tempPlayer.primaryClanID = tempArray[1];
                                break;
                            case "loccountrycode":
                                tempPlayer.locCountryCode = tempArray[1];
                                break;
                            case "locstatecode":
                                tempPlayer.locStateCode = tempArray[1];
                                break;
                            case "realname":
                                tempPlayer.realName = tempArray[1];
                                break;
                        }
                    }
                    if (Suspect.Instance.playerData == null) { Suspect.Instance.playerData = tempPlayer; }
                }
            }
        }
        public string returnApiReply(string uri)
        {
            try {
                HttpClient client;
                HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate };
                client = new HttpClient(handler);

                string reply = null;
                client.BaseAddress = new Uri(uri);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

                reply = response.Content.ReadAsStringAsync().Result;
                return reply;
            }
            catch {  return returnApiReply(uri);   }
            
        }//Returns the reply of the result.
        internal bool forbiddenCheck(string APIKey, string steamID)
        {
            try {
                HttpClient client;
                HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate };
                client = new HttpClient(handler);

                string reply = null;
                client.BaseAddress = new Uri("https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + APIKey + "&steamids=" + steamID);
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

                if (response.ReasonPhrase == "Forbidden") { return true; }
                return false;
            } catch { return true; }
            
        }//Checks to see if provided APIKey is valid.

        #region Archived Code just incase

        //public void bannedPlayersThreads(string threadType) {
        //    foreach(Player bannedFriend in Suspect.Instance.suspectList) {

        //        if (threads.Count == 50) { waitForAllThreads(); }
        //        Thread thread = new Thread(() =>
        //        {
        //            if (threadType == "level") { handleLevel(returnApiReply(updateURI("level", bannedFriend.steamID)), bannedFriend); }
        //            else if (threadType == "summary") { handleSummary(returnApiReply(updateURI("summary", bannedFriend.steamID)), bannedFriend); }
        //            else if (threadType == "gameList") { handleGameList(returnApiReply(updateURI("gameList", bannedFriend.steamID)), bannedFriend); }
        //            else if (threadType == "recentGames") { handleRecentGameList(returnApiReply(updateURI("recentGames", bannedFriend.steamID)), bannedFriend); }
        //            else if (threadType == "UNIX") { }
        //            else if (threadType == "Analysis") { }
        //        });
        //        threads.Add(thread); thread.Start();
        //    }
        //    waitForAllThreads();
        //}//Deals with all threads using the suspectList - Summaries, level, game, recentgame requests. (UNIX Timestamp creations aswell as playerAnalysis) - may move to a different class

        //private void handleBans(string result, string steamID)
        //{
        //    Player tempPlayer = new Player();
        //    if(result != "{}" && result.Contains("VACBanned\":true")) {

        //        #region Seperating Info from APIResponse
        //        result = result.Remove(0, 25);
        //        result = result.Remove((result.Length - 4), 4);
        //        result = result.Replace("\"", "");
        //        #endregion
        //        string[] splitResponse = result.Split(',');
        //        tempPlayer.steamID = splitResponse[0];
        //        #region Extract Information
        //        for (int i = 0; i < splitResponse.Length; i++)
        //        {
        //            string[] tempArray = splitResponse[i].Split(':', 2);
        //            switch (tempArray[0])
        //            {
        //                case "VACBanned":
        //                    tempPlayer.vacBanned = bool.Parse(tempArray[1]);
        //                    break;
        //                case "numberOfVACBans":
        //                    tempPlayer.numberOfVACBans = int.Parse(tempArray[1]);
        //                    break;
        //                case "DaysSinceLastBan":
        //                    tempPlayer.daysSinceLastBan = int.Parse(tempArray[1]);
        //                    break;
        //            }
        //        }
        //        Suspect.Instance.suspectList.Add(tempPlayer);  //If they player has a VAC ban, they are added to the suspect list.
        //        #endregion
        //    } //Public account
        //    else if (result.Contains("429 Too Many Requests"))
        //    {
        //        Thread.Sleep(3000);
        //        handleBans(returnApiReply(updateURI("bans", steamID)), steamID);
        //    }//If too many requests, waits a second and then redoes the request.
        //}

        //public void startThread(string threadType, string xApiKey, string xSteamID) {

        //    ApiKey = xApiKey; steamID = xSteamID;
        //    if(threadType == "createPlayer") {
        //        Thread thread = new Thread(() =>
        //        {
        //            Player tempPlayer = handleBans(returnApiReply(updateURI("bans")));
        //            if(tempPlayer.vacBanned || Suspect.Instance.playerData == null) {
        //                tempPlayer = handleSummary(returnApiReply(updateURI("summary")), tempPlayer);
        //                //tempPlayer = handleLevel(returnApiReply(updateURI("level")), tempPlayer);
        //                //tempPlayer = handleGameList(returnApiReply(updateURI("gameList")), tempPlayer);
        //                //tempPlayer = handleRecentGameList(returnApiReply(updateURI("recentGames")), tempPlayer);
        //                if (Suspect.Instance.playerData == null) {
        //                    Suspect.Instance.playerData = tempPlayer;
        //                    handleFriends(returnApiReply(updateURI("friends")));
        //                }
        //                else {
        //                    Suspect.Instance.suspectList.Add(tempPlayer); }
        //            }//If player is banned or if player info is the suspect.
        //        });
        //        thread.Name = "createPlayer"; threads.Add(thread); thread.Start();
        //    }//Creates a player object for player provided from steamID list and adds to suspectList if they have a vac ban, or assigns to suspect if data belongs to suspect
        //    else if (threadType == "friendsList") {
        //        Thread thread = new Thread(() =>
        //        {
        //            handleFriends(returnApiReply(updateURI("friends")));
        //        });
        //        threads.Add(thread);
        //        thread.Start();
        //        thread.Join();
        //    }//Adds friends to steamID List

        //}

        //public void gatherSuspects()
        //{
        //    for (int i = 0; i < Suspect.Instance.steamIDList.Count; i++)
        //    {
        //        string steamID = Suspect.Instance.steamIDList[i];
        //        if (threads.Count == 50) { waitForAllThreads(); }
        //        Thread thread = new Thread(() =>
        //        {
        //            handleBans(returnApiReply(updateURI("bans", steamID)), steamID);
        //        });
        //        threads.Add(thread); thread.Start();
        //    }
        //    waitForAllThreads();
        //}//Does a ban query on every steamID in the SteamID list, if they have a ban, all information is stored and they are added to the suspect list.
        #endregion
    }
}
