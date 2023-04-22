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
                    Suspect.Instance.resetPlayer();//Incase this is a request following another request, clears the data of the previous suspect.

                    #region Getting Information on Suspect and assigning it to playerData
                    ResultTxtbox.Text = "Gathering Suspect Data...\r\n"; updateProgressBar();
                    apiRequest.assignToSuspectData();
                    if(Suspect.Instance.playerData == null) { ResultTxtbox.Text += "Could not retrieve Suspect Summary, Account is private.\r\n"; return; }
                    else if (Suspect.Instance.playerData.friendsList.Count == 0) { ResultTxtbox.Text += "Could not retrieve Friends List, Friends List must not be public.\r\n"; return; }
                    else { ResultTxtbox.Text += "Gathered.\r\n"; }

                    #endregion

                    #region Get friends of friends using Threads, add it to suspects steamIDlist.
                    ResultTxtbox.Text += "\r\nGathering Friends of Friends...\r\n"; updateProgressBar();
                    apiRequest.getFriendsofFriends(Suspect.Instance.steamIDList);
                    Suspect.Instance.steamIDList = Suspect.Instance.steamIDList.Distinct().ToList();
                    ResultTxtbox.Text += "Gathered " + Suspect.Instance.steamIDList.Count + " accounts. \r\n";
                    #endregion

                    #region Iterate through steamIDList, if they have a ban, add them to suspectList.
                    ResultTxtbox.Text += "\r\nGathering Banned Friends...\r\n"; updateProgressBar();
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
                    ResultTxtbox.Text += "\r\nGathering BannedPlayers Steam Levels...\r\n"; updateProgressBar();
                    apiRequest.gatherBPLevel();
                    ResultTxtbox.Text += "Gathered.\r\n";
                    #endregion

                    #region Getting Banned Players FriendsLists
                    ResultTxtbox.Text += "\r\nGathering BannedPlayers FriendsLists...\r\n"; updateProgressBar();
                    apiRequest.gatherBPFriendsLists();
                    ResultTxtbox.Text += "Gathered.\r\n";
                    #endregion

                    #region Iterating BannedPlayers and assigning UNIX Timestamp for most recent ban.
                    ResultTxtbox.Text += "\r\nCaluclating difference between Ban time and Suspect creation time...\r\n"; updateProgressBar();
                    calculateTimestamps();
                    ResultTxtbox.Text += "Caluclated.\r\n";
                    #endregion

                    #region Assigning BannedPlayers a Suspect Rating
                    ResultTxtbox.Text += "\r\nEvaluating all banned players, assigning Suspect Ratings...\r\n"; updateProgressBar();
                    PlayerAnalysis playerAnalysis = new PlayerAnalysis();
                    playerAnalysis.startAnalysis();
                    Suspect.Instance.suspectList = Suspect.Instance.suspectList.OrderBy(o => o.suspectRating).ToList();
                    Suspect.Instance.suspectList.Reverse();
                    ResultTxtbox.Text += "Completed.\r\n";
                    #endregion

                    #region Display Results
                    displayResults(); updateProgressBar();
                    #endregion

                }//Checks validty of steamAPI Key
                else { ResultTxtbox.Text = "---Forbidden!--- \r\nPlease ensure that the APIKey is correct.\r\nThe APIKey you have entered could be invalid!\r\nIf you have just done a big request, please ensure you wait before sending another request."; }

            }//Information Entered
            else { ResultTxtbox.Text = "---Analysis Failure!--- \r\nPlease ensure that the APIKey and SteamID are entered.\r\nIf they are already entered, ensure they are correct."; }
        }

        private void displayResults()
        {
            ResultTxtbox.Clear(); ResultTxtbox.Text += "---Analysis Completed!--- \r\nThe following is a list of all banned accounts, from most to least suspicous.\r\n" +
                "The format of the results are Name: Rating (SteamID) - Privacy, The higher the Rating, the more suspicous the account is. \r\n" +
                "Bear in mind ALL of these accounts have a VAC ban.\r\n\r\n";
            foreach (Player bannedPlayer in Suspect.Instance.suspectList)
            {
                ResultTxtbox.Text += bannedPlayer.personaName + ": " + bannedPlayer.suspectRating + " (" + bannedPlayer.steamID + ") - ";
                if (bannedPlayer.communityVisibilityState == 3) { ResultTxtbox.Text += "Public\r\n"; }
                else if (bannedPlayer.communityVisibilityState == 2) { ResultTxtbox.Text += "Restricted To Friends\r\n"; }
                else { ResultTxtbox.Text += "Private\r\n"; }
            }
        }//Displays Results

        public void updateProgressBar() { progressBar1.Value += 10; progressBar1.Refresh(); Application.DoEvents(); } //Used to increment the progress bar.
        public void calculateTimestamps()
        {
            foreach (Player player in Suspect.Instance.suspectList)
            {
                int secondsSinceBan = player.daysSinceLastBan * 86400;
                player.unixTimestampOfRecentBan = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - secondsSinceBan;
            }
        }//Calculates the time difference between date of most recent ban and the account creation time of the suspect.

        private void Savebtn_Click(object sender, EventArgs e)
        {
            if (ResultTxtbox.Text.Contains("---Analysis Completed!---"))
            {
                string fileName = Suspect.Instance.playerData.personaName + " - " + DateTime.Now.ToString() + ".txt";
                fileName = fileName.Replace("/", "-"); fileName = fileName.Replace(':', '-');
                StreamWriter sw = new StreamWriter(fileName);
                sw.Write(ResultTxtbox.Text);
                sw.Close();
                ResultTxtbox.Clear();
                ResultTxtbox.Text = "---Analysis Saved!---\r\nIt has been saved under " + fileName;
            }
        }
    }
}