using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayerInvestigatorV2
{
    class PlayerAnalysis : SteamPlayerInvestigator
    {
        public PlayerAnalysis() { }
        public List<Thread> threads = new List<Thread>();
        public void evaluateUNIX(Player bannedPlayer) {
            int differenceInUnix = Suspect.Instance.playerData.timeCreated - bannedPlayer.unixTimestampOfRecentBan;
            if (differenceInUnix <= 604800) { bannedPlayer.suspectRating += 100; }//A week
            else if (differenceInUnix <= 1209600) { bannedPlayer.suspectRating += 80; }//1-2 weeks
            else if (differenceInUnix <= 1814400) { bannedPlayer.suspectRating += 60; }//2-3 weeks
            else if (differenceInUnix <= 2629743) { bannedPlayer.suspectRating += 40; }//3-4 weeks
            else if (differenceInUnix <= 5259486) { bannedPlayer.suspectRating += 20; }//1-2 months
            else if (differenceInUnix <= 7889229) { bannedPlayer.suspectRating += 10; }//2-3 months
        }
        public void evaluatePlayerLevel(Player bannedPlayer) { 
            if(bannedPlayer.steamLevel == 0){ bannedPlayer.suspectRating += 10; }
            else if(bannedPlayer.steamLevel == 1) { bannedPlayer.suspectRating += 8; }
            else if (bannedPlayer.steamLevel == 2) { bannedPlayer.suspectRating += 6; }
            else if (bannedPlayer.steamLevel == 3) { bannedPlayer.suspectRating += 4; }
            else if (bannedPlayer.steamLevel == 4) { bannedPlayer.suspectRating += 2; }
            else if (bannedPlayer.steamLevel == 5) { bannedPlayer.suspectRating += 1; }
        }
        public void compareGameList(Player bannedPlayer) { }
        public void compareRecentGames(Player bannedPlayer) { }
        public void comparePrimaryClan(Player bannedPlayer) { }
        public void compareAvatar(Player bannedPlayer) { }
        public void steamLevelAnalysis(Player bannedPlayer) { }
        public void compareLocCodes(Player bannedPlayer) { }
        public void compareRealName(Player bannedPlayer) { 
            if (bannedPlayer.realName == Suspect.Instance.playerData.realName) { 
                
            } }

        public void startAnalysis()
        {
            foreach(Player bannedPlayer in Suspect.Instance.suspectList)
            {
                Thread thread = new Thread(() =>
                {
                    evaluateUNIX(bannedPlayer);
                    evaluatePlayerLevel(bannedPlayer);
                    compareAvatar(bannedPlayer);
                    comparePrimaryClan(bannedPlayer);
                    compareGameList(bannedPlayer);
                    compareRecentGames(bannedPlayer);
                });
                threads.Add(thread); thread.Start();
            }
            waitForAllThreads();
        }
        private void waitForAllThreads()
        {
            while (threads.Count != 0)
            {
                for (int i = 0; i < threads.Count; i++)
                {
                    if (threads[i].ThreadState == ThreadState.Stopped) { threads.RemoveAt(i); i--; }
                }
            }
        }//Waits for all threads to be done.
    }
}
