using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayerInvestigatorV2
{
    public class Suspect
    {
        public List<Player> suspectList {  get; set; }
        public List<string> steamIDList { get; set; }
        public Player playerData { get; set; }
    }
}
