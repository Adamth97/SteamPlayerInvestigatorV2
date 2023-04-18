namespace SteamPlayerInvestigatorV2
{
    public partial class SteamPlayerInvestigator : Form
    {
        private SteamAPI apiRequest { get; set; }
        public SteamPlayerInvestigator()
        {
            InitializeComponent();
        }

        private async void AnalyseBtn_Click(object sender, EventArgs e)
        {
            ResultTxtbox.Clear(); progressBar1.Value = 0; Application.DoEvents();//Clears Result Box
            string APIKey = ApiKeyTxtbox.Text; string SteamID = SteamIDTxtbox.Text;
            if (APIKey != "" && SteamID != "")
            {
                apiRequest = new SteamAPI(APIKey, SteamID);
                if (apiRequest.forbiddenCheck(APIKey, SteamID) == false)
                {

                    #region Getting Information on Suspect and assigning it to playerData
                    ResultTxtbox.Text = "Gathering Suspect Data...\r\n"; updateProgressBar();
                    apiRequest.assignToSuspectData();
                    ResultTxtbox.Text += "Gathered.\r\n";
                    #endregion

                    #region Get friends of friends using Threads, add it to suspects steamIDlist.
                    ResultTxtbox.Text += "\r\nGathering Friends of Friends...\r\n"; updateProgressBar();
                    apiRequest.getFriendsofFriends(Suspect.Instance.steamIDList);
                    Suspect.Instance.steamIDList = Suspect.Instance.steamIDList.Distinct().ToList();
                    ResultTxtbox.Text += "Gathered.\r\n";

                    if (Suspect.Instance.steamIDList.Count > 1000) { ResultTxtbox.Text += "\r\nSteamID List greater than 1000, Suspect gathering may take longer.\r\n"; }
                    else if (Suspect.Instance.steamIDList.Count > 5000) { ResultTxtbox.Text += "\r\nSteamID List greater than 5000, Suspect gathering will take longer than normal.\r\n"; }
                    else if (Suspect.Instance.steamIDList.Count > 10000) { ResultTxtbox.Text += "\r\nSteamID List greater than 10000, Suspect gathering will take long, please expect a delay in progress.\r\n"; }
                    ResultTxtbox.Text += "The delay is due to SteamAPI, a limit of requests can be made by the same person each minute, this is to prevent DDOS attacks.\r\n";
                    ResultTxtbox.Text += "This software has to send lots of requests, therefore a delay has to be put in place to prevent being blocked.\r\n\r\n";
                    #endregion

                    #region Iterate through steamIDList, if they have a ban, add them to suspectList.
                    ResultTxtbox.Text += "Gathering Banned Friends...\r\n"; updateProgressBar();
                    apiRequest.gatherSuspects();
                    Suspect.Instance.suspectList = Suspect.Instance.suspectList.Distinct().ToList();
                    ResultTxtbox.Text += "Gathered " + Suspect.Instance.suspectList.Count + " suspects.\r\n";
                    #endregion

                    #region Getting Banned Players summaries
                    ResultTxtbox.Text += "\r\nGathering BannedPlayers Summaries...\r\n"; updateProgressBar();
                    apiRequest.gatherSummaries();
                    ResultTxtbox.Text += "Gathered.\r\n";
                    #endregion

                    #region Getting Banned Players gameList
                    ResultTxtbox.Text += "\r\nGathering BannedPlayers GameList...\r\n"; updateProgressBar();
                    apiRequest.gatherBPGameList();  
                    ResultTxtbox.Text += "Gathered.\r\n";
                    #endregion

                    #region Getting Banned Players RecentGames
                    ResultTxtbox.Text += "\r\nGathering BannedPlayers RecentGames...\r\n"; updateProgressBar();
                    apiRequest.gatherBPRecentGames();
                    ResultTxtbox.Text += "Gathered.\r\n";
                    #endregion

                    #region Getting Banned Players steamLevel
                    ResultTxtbox.Text += "\r\nGathering BannedPlayers steamLevels...\r\n"; updateProgressBar();
                    //await apiRequest.gatherSuspects();
                    ResultTxtbox.Text += "Gathered.\r\n";
                    #endregion

                    #region Iterating BannedPlayers and assigning UNIX Timestamp for most recent ban.
                    ResultTxtbox.Text += "\r\nCaluclating difference between Ban time and Suspect creation time...\r\n"; updateProgressBar();
                    //calculateTimestamps();
                    ResultTxtbox.Text += "Caluclated.\r\n";
                    #endregion

                    #region Assigning BannedPlayers a Suspect Rating
                    ResultTxtbox.Text += "\r\nEvaluating all banned players, assigning suspect ratings...\r\n"; updateProgressBar();
                    //evaluateBannedPlayers();
                    ResultTxtbox.Text += "Completed.\r\n";
                    #endregion

                    #region Display Results
                    updateProgressBar();
                    #endregion

                }//Checks validty of steamAPI Key
                else { ResultTxtbox.Text = "---Forbidden!--- \r\nPlease ensure that the APIKey is correct.\r\nThe APIKey you have entered could be invalid!\r\nIf you have just done a big request, please ensure you wait before sending another request."; }

            }//Information Entered
            else { ResultTxtbox.Text = "---Analysis Failure!--- \r\nPlease ensure that the APIKey and SteamID are entered.\r\nIf they are already entered, ensure they are correct."; }
        }

        public void updateProgressBar() { progressBar1.Value += 10; progressBar1.Refresh(); Application.DoEvents(); } //Used to increment the progress bar.
    }
}