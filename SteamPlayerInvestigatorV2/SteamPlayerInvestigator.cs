namespace SteamPlayerInvestigatorV2
{
    public partial class SteamPlayerInvestigator : Form
    {
        private SteamAPI apiRequest { get; set; }
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
                    #region Getting Information on Suspect and assigning it to playerData
                    apiRequest.startThread("createPlayer", APIKey, SteamID); progressBar1.Value = 100;
                    waitForActiveThreads(); //Waits for all threads to be done.
                    ResultTxtbox.Text += "Suspected Data Retrieved."; Thread.Sleep(3000);  progressBar1.Value = 0;
                    #endregion
                    #region Adding friends of friends to steamID List.
                    foreach (string friend in Suspect.Instance.steamIDList)
                    {
                        apiRequest.startThread("friendsList", APIKey, friend); progressBar1.Value += ( 100 / Suspect.Instance.steamIDList.Count); 
                    }
                    waitForActiveThreads();
                    ResultTxtbox.Text += "Friends Of Friends Retrieved."; progressBar1.Value = 0;
                    #endregion
                    //Check to see if the suspect is private, if so, end the analysis here.
                    
                }//Checks validty of steamAPI Key
                else { ResultTxtbox.Text = "---Forbidden!--- \r\nPlease ensure that the APIKey is correct.\r\nThe APIKey you have entered is invalid!"; }

            }//Information Entered
            else { ResultTxtbox.Text = "---Analysis Failure!--- \r\nPlease ensure that the APIKey and SteamID are entered.\r\nIf they are already entered, ensure they are correct."; }
        }

        private void waitForActiveThreads()
        {
            bool active = true;
            while (active)
            {
                foreach (Thread t in apiRequest.threads)
                {
                    active = t.IsAlive;
                }
            }
        }//Waits for all threads to be done.
    }
}