using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayerInvestigatorV2
{
    public sealed class SteamAPI : SteamPlayerInvestigator
    {
        private static SteamAPI instance;
        private static readonly object padlock = new object();
        private List<Thread> threads = new List<Thread>();

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

        public void startThread(string uri, string request, Player player) {
            Thread thread = new Thread(() =>
            {
                string result = returnApiReply(uri);
                if (request == "summary") { handleSummary(result, player); }
                else if (request == "friends") { handleFriends(result, player); }
                else if (request == "bans") { handleBans(result, player); }
                else if (request == "level") { handleLevel(result, player); }
                else if (request == "gamelist") { handleGameList(result, player); }
                else { handleRecentGameList(result, player); } //RecentGames Request
            });
            threads.Add(thread);
            thread.Start();
        }

        private void handleRecentGameList(string result, Player player)
        {
            throw new NotImplementedException();
        }

        private void handleGameList(string result, Player player)
        {
            throw new NotImplementedException();
        }

        private void handleLevel(string result, Player player)
        {
            throw new NotImplementedException();
        }

        private void handleBans(string result, Player findThisPlayer)
        {
            throw new NotImplementedException();
        }

        private void handleFriends(string result, Player findThisPlayer)
        {
            throw new NotImplementedException();
        }

        private void handleSummary(string summary, Player findThisPlayer)
        {
            throw new NotImplementedException();
        }

        public string returnApiReply(string uri)
        {
            return string.Empty;
        }

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
