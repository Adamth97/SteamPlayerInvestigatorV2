namespace SteamPlayerInvestigatorV2
{
    public partial class SteamPlayerInvestigator : Form
    {
        private SteamAPI apiRequest { get; set; }
        public Suspect suspect { get; set; }
        public SteamPlayerInvestigator()
        {
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
                    Player suspectData = new Player();
                    apiRequest.startThread(returnURI("summary", APIKey, SteamID), "summary", suspectData);
                    //on suspect - player summary, friends, games, recent games
                }
                else { ResultTxtbox.Text = "---Forbidden!--- \r\nPlease ensure that the APIKey is correct.\r\nThe APIKey you have entered is invalid!"; }

            }//Information Entered
            else { ResultTxtbox.Text = "---Analysis Failure!--- \r\nPlease ensure that the APIKey and SteamID are entered.\r\nIf they are already entered, ensure they are correct."; }
        }

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