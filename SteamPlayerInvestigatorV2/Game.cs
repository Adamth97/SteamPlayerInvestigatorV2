using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayerInvestigatorV2
{
    public class Game
    {
        public int gameID { get; set; }
        public int playTime { get; set; }
        public int lastPlayed { get; set; }
        public int playtimeForever { get; set; }
    }
}
