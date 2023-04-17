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
                    ResultTxtbox.Text = "Gathering Suspect Data...\r\n"; progressBar1.Value += 20; progressBar1.Refresh(); Application.DoEvents();
                    Player suspectData = apiRequest.returnSuspectData();
                    Suspect.Instance.playerData = suspectData;
                    ResultTxtbox.Text += "Gathered.\r\n";
                    #endregion

                    #region Get friends of friends using Threads, add it to suspects steamIDlist.
                    ResultTxtbox.Text += "Gathering Friends of Friends...\r\n"; progressBar1.Value += 20; progressBar1.Refresh(); Application.DoEvents();
                    await apiRequest.getFriendsofFriends(Suspect.Instance.steamIDList);
                    Suspect.Instance.steamIDList = Suspect.Instance.steamIDList.Distinct().ToList();
                    ResultTxtbox.Text += "Gathered.\r\n";

                    if(Suspect.Instance.steamIDList.Count > 1000) { ResultTxtbox.Text += "SteamID List greater than 1000, Suspect gathering may take longer.\r\n"; }
                    else if (Suspect.Instance.steamIDList.Count > 5000) { ResultTxtbox.Text += "SteamID List greater than 5000, Suspect gathering will take longer than normal.\r\n"; }
                    else if (Suspect.Instance.steamIDList.Count > 10000) { ResultTxtbox.Text += "SteamID List greater than 10000, Suspect gathering will take long, please expect a delay in progress.\r\n"; }
                    ResultTxtbox.Text += "The delay is due to SteamAPI, a limit of requests can be made by the same person each minute, this is to prevent DDOS attacks.\r\n";
                    ResultTxtbox.Text += "This software has to send lots of requests, therefore a delay has to be put in place to prevent being blocked.\r\n";
                    #endregion

                    #region Iterate through steamIDList, if they have a ban, add them to suspectList.
                    ResultTxtbox.Text += "Gathering Suspects...\r\n"; progressBar1.Value += 20; progressBar1.Refresh(); Application.DoEvents();
                    await apiRequest.gatherSuspects();
                    Suspect.Instance.suspectList = Suspect.Instance.suspectList.Distinct().ToList();
                    ResultTxtbox.Text += "Gathered " + Suspect.Instance.suspectList.Count + " suspects.\r\n";
                    #endregion

                    #region Getting suspects data
                    ResultTxtbox.Text += "Gathering Suspects data...\r\n"; progressBar1.Value += 20; progressBar1.Refresh(); Application.DoEvents();
                    //await apiRequest.gatherSuspects();
                    //Suspect.Instance.steamIDList = Suspect.Instance.steamIDList.Distinct().ToList();
                    ResultTxtbox.Text += "Gathered.\r\n";
                    #endregion






                    //#region Adding friends of friends to steamID List.
                    //int idCount = Suspect.Instance.steamIDList.Count;
                    //for (int i = 0; i < idCount; i++)
                    //{
                    //    if (apiRequest.threads.Count == 15)
                    //    {
                    //        while (apiRequest.threads.Count != 0) { waitForActiveThreads(); }
                    //    }
                    //    //apiRequest.startThread("friendsList", APIKey, Suspect.Instance.steamIDList[i]);
                    //}

                    //while (apiRequest.threads.Count != 0) { waitForActiveThreads(); }
                    //ResultTxtbox.Text += "Friends Of Friends Retrieved.";
                    //Suspect.Instance.steamIDList = Suspect.Instance.steamIDList.Distinct().ToList();
                    //#endregion

                    //#region Adding to SuspectList
                    //for (int i = 0; i < Suspect.Instance.steamIDList.Count; i++)
                    //{
                    //    if (apiRequest.threads.Count == 15) { 
                    //        while (apiRequest.threads.Count != 0) { waitForActiveThreads(); } }
                    //    //apiRequest.startThread("createPlayer", APIKey, Suspect.Instance.steamIDList[i]);

                    //    //remove duplicates from steamIDList here
                    //}
                    //while (apiRequest.threads.Count != 0) { waitForActiveThreads(); }
                    //ResultTxtbox.Text += "\r\nSuspect List Created.";
                    //#endregion
                    ////Check to see if the suspect is private, if so, end the analysis here.

                }//Checks validty of steamAPI Key
                else { ResultTxtbox.Text = "---Forbidden!--- \r\nPlease ensure that the APIKey is correct.\r\nThe APIKey you have entered could be invalid!\r\nIf you have just done a big request, please ensure you wait before sending another request."; }

            }//Information Entered
            else { ResultTxtbox.Text = "---Analysis Failure!--- \r\nPlease ensure that the APIKey and SteamID are entered.\r\nIf they are already entered, ensure they are correct."; }
        }

        //private void waitForActiveThreads()
        //{
        //    bool active = true;
        //    while (active)
        //    {
        //        active = false;
        //        for(int i = 0; i<apiRequest.threads.Count;i++)
        //        {
        //            if (apiRequest.threads[i].IsAlive) { active = true; }
        //            else{ apiRequest.threads.Remove(apiRequest.threads[i]); i--; }
        //        }
        //    }
        //}//Waits for all threads to be done.
    }
}