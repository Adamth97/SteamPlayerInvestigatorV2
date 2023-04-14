using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayerInvestigatorV2
{
    public sealed class Suspect
    {
        private static Suspect instance = null;
        private static readonly object padlock = new object();
        Suspect() { }
        public static Suspect Instance
        {
            get { 
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Suspect();
                    }
                    return instance;
                }
            }
        }
        public List<Player> suspectList {  get; set; }
        public List<string> steamIDList { get; set; }
        public Player playerData { get; set; }
    }
}
