namespace SteamPlayerInvestigatorV2
{
    public partial class SteamPlayerInvestigator : Form
    {
        private SteamAPI apiRequest { get; set; }
        public Suspect suspect { get; set; }
        public SteamPlayerInvestigator()
        {
            suspect = Suspect.Instance;
            InitializeComponent();
        }

        private void AnalyseBtn_Click(object sender, EventArgs e)
        {
            ResultTxtbox.Clear(); //Clears Result Box
            string APIKey = ApiKeyTxtbox.Text; string SteamID = SteamIDTxtbox.Text;
            if (APIKey != "" && SteamID != "")
            {
                apiRequest = SteamAPI.Instance;
                if (apiRequest.forbiddenCheck(APIKey, SteamID) == false)
                {
                    #region Getting Information on Suspect
                    Player suspectData = new Player();
                    apiRequest.startThread(returnURI("summary", APIKey, SteamID), "summary", suspectData);
                    apiRequest.startThread(returnURI("friends", APIKey, SteamID), "summary", suspectData);
                    apiRequest.startThread(returnURI("gameList", APIKey, SteamID), "summary", suspectData);
                    apiRequest.startThread(returnURI("recentGames", APIKey, SteamID), "summary", suspectData);
                    waitForActiveThreads(); //Waits for all threads to be done.
                    ResultTxtbox.Text = "---Stage 1 Completed!--- \r\nSuspected Data Retrieved.";
                    //Check to see if the suspect is private, if so, end the analysis here.
                    #endregion
                }//Checks validty of steamAPI Key
                else { ResultTxtbox.Text = "---Forbidden!--- \r\nPlease ensure that the APIKey is correct.\r\nThe APIKey you have entered is invalid!"; }

            }//Information Entered
            else { ResultTxtbox.Text = "---Analysis Failure!--- \r\nPlease ensure that the APIKey and SteamID are entered.\r\nIf they are already entered, ensure they are correct."; }
        }

        private void waitForActiveThreads()
        {
            while (true)
            {
                foreach(Thread t in apiRequest.threads)
                {
                    if(t.IsAlive) { continue; }
                    break;
                }
            }
        }//Waits for all threads to be done.

        private string returnURI(string requestType, string ApiKey, string steamID)
        {
            string uri = null;
            switch (requestType) {
                case "summary":
                    uri = "https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + ApiKey + "&steamids=" + steamID;
                    break;
                case "friends":

                    break;
                case "bans":

                    break;
                case "level":

                    break;
                case "gameList":

                    break;
                default:

                    break;
            }
            return uri;
        }
    }
}