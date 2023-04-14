namespace SteamPlayerInvestigatorV2
{
    public partial class SteamPlayerInvestigator : Form
    {
        SteamAPI apiRequest { get; set; }
        public SteamPlayerInvestigator()
        {
            apiRequest = new SteamAPI();
            InitializeComponent();
        }

        private void AnalyseBtn_Click(object sender, EventArgs e)
        {
            ResultTxtbox.Clear(); //Clears Result Box
            string APIKey = ApiKeyTxtbox.Text; string SteamID = SteamIDTxtbox.Text;
            if (APIKey != "" && SteamID != "")
            {

            }//Information Entered
            else { ResultTxtbox.Text = "---Analysis Failure!--- \r\nPlease ensure that the APIKey and SteamID are entered.\r\nIf they are already entered, ensure they are correct."; }
        }
    }
}