using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayerInvestigatorV2
{
    public class Player
    {
        public Player() { gameList = new List<Game>(); recentlyPlayed = new List<Game>(); friendsList = new List<string>(); } 

        #region Account Info get; set;
        public string steamID { get; set; }
        public string personaName { get; set; }
        public string avatar { get; set; }
        public string realName { get; set; }
        public string primaryClanID { get; set; }
        public int communityVisibilityState { get; set; }
        public int timeCreated { get; set; }
        public int steamLevel { get; set; }
        #endregion
        #region VacBan Info get; set;
        public bool vacBanned { get; set; }
        public int numberOfVACBans { get; set; }
        public int daysSinceLastBan { get; set; }
        public int unixTimestampOfRecentBan { get; set; }
        public int suspectRating { get; set; }
        #endregion
        #region Game Info get; set
        public List<Game> gameList { get; set; }
        public List<Game> recentlyPlayed { get; set; }
        #endregion
        #region Misc get; set;
        public string locCountryCode { get; set; }
        public string locStateCode { get; set; }
        public List<string> friendsList { get; set; }
        #endregion

    }
}
