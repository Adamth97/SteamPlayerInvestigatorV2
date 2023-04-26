using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SteamPlayerInvestigatorV2
{
    class PlayerAnalysis : SteamPlayerInvestigator
    {
        Suspect suspect;
        public PlayerAnalysis() { suspect = Suspect.Instance; }
        public List<Thread> threads = new List<Thread>();

        /// <summary>
        /// Banned players suspect ratings are added to in intervals depending on how suspicous the data is.
        /// Suspect rating additions range from 1-100, 100 being very suspicous and 1 being very slightly.
        /// This results in a rating at the end of analysis, the higher the rating, the more suspicous the account, the more likely it is to be the Suspects main account.
        /// </summary>
        /// <param name="bannedPlayer"></param>

        public void percentageSimilaritySuspectRatings(Player bannedPlayer, double percentageSimilarity) {
            if (percentageSimilarity == 100) { bannedPlayer.suspectRating += 25; }
            else if (percentageSimilarity >= 80) { bannedPlayer.suspectRating += 20; }
            else if (percentageSimilarity >= 60) { bannedPlayer.suspectRating += 15; }
            else if (percentageSimilarity >= 40) { bannedPlayer.suspectRating += 10; }
            else if (percentageSimilarity >= 20) { bannedPlayer.suspectRating += 5; }
        }
        public void friendsListAnalysis(Player bannedPlayer) {
            double similarity = suspect.playerData.friendsList.Intersect(bannedPlayer.friendsList).Count();
            similarity = similarity / suspect.playerData.friendsList.Count();
            double percentageSimilarity = similarity * 100;
            if (percentageSimilarity == 100) { bannedPlayer.suspectRating += 75; }
            else if (percentageSimilarity >= 90) { bannedPlayer.suspectRating += 70; }
            else if (percentageSimilarity >= 80) { bannedPlayer.suspectRating += 65; }
            else if (percentageSimilarity >= 70) { bannedPlayer.suspectRating += 60; }
            else if (percentageSimilarity >= 60) { bannedPlayer.suspectRating += 50; }
            else if (percentageSimilarity >= 50) { bannedPlayer.suspectRating += 40; }
            else if (percentageSimilarity >= 40) { bannedPlayer.suspectRating += 30; }
            else if (percentageSimilarity >= 30) { bannedPlayer.suspectRating += 20; }
            else if (percentageSimilarity >= 20) { bannedPlayer.suspectRating += 10; }
            else if (percentageSimilarity >= 10) { bannedPlayer.suspectRating += 5; }
        }//Compares the percentage similarity between suspects friends and banned players friends.
        public void evaluateUNIX(Player bannedPlayer) {
            int differenceInUnix = suspect.playerData.timeCreated - bannedPlayer.unixTimestampOfRecentBan;
            if (differenceInUnix > 0) {
                if (differenceInUnix <= 604800) { bannedPlayer.suspectRating += 100; }//A week
                else if (differenceInUnix <= 1209600) { bannedPlayer.suspectRating += 80; }//1-2 weeks
                else if (differenceInUnix <= 1814400) { bannedPlayer.suspectRating += 60; }//2-3 weeks
                else if (differenceInUnix <= 2629743) { bannedPlayer.suspectRating += 40; }//3-4 weeks
                else if (differenceInUnix <= 5259486) { bannedPlayer.suspectRating += 20; }//1-2 months
                else if (differenceInUnix <= 7889229) { bannedPlayer.suspectRating += 10; }//2-3 months
            }
        }//Adds to suspect rating of banned player based on suspect account creation in comparison to bannedPlayer most recent ban
        public void evaluatePlayerLevel(Player bannedPlayer) { 
            if(bannedPlayer.communityVisibilityState == 3)
            {
                if (bannedPlayer.steamLevel == 0) { bannedPlayer.suspectRating += 10; }
                else if (bannedPlayer.steamLevel == 1) { bannedPlayer.suspectRating += 8; }
                else if (bannedPlayer.steamLevel == 2) { bannedPlayer.suspectRating += 6; }
                else if (bannedPlayer.steamLevel == 3) { bannedPlayer.suspectRating += 4; }
                else if (bannedPlayer.steamLevel == 4) { bannedPlayer.suspectRating += 2; }
                else if (bannedPlayer.steamLevel == 5) { bannedPlayer.suspectRating += 1; }
            }
        }//Adds to suspect rating of banned player based on Steam Level
        public void comparePersonaName(Player bannedPlayer) {
            if(bannedPlayer.personaName == null) { return; }

            int numberOfChangesRequired = 0, iterate = 0;

            if(bannedPlayer.personaName.Length > suspect.playerData.personaName.Length) { numberOfChangesRequired += bannedPlayer.personaName.Length - suspect.playerData.personaName.Length;
                iterate = suspect.playerData.personaName.Length; } //BannedPlayer persona longer then suspects
            else { numberOfChangesRequired += suspect.playerData.personaName.Length - bannedPlayer.personaName.Length;
                iterate = bannedPlayer.personaName.Length; }//Suspect persona longer than bannedPlayer

            for (int i = 0; i < iterate; i++)
            {
                if (bannedPlayer.personaName[i] != suspect.playerData.personaName[i]) { numberOfChangesRequired++; }
            }//The more similar the names, the less number of changes required.

            if(numberOfChangesRequired == 0) { bannedPlayer.suspectRating += 75; }//Both can have the same persona name
            else if (numberOfChangesRequired == 1) { bannedPlayer.suspectRating += 60; }
            else if (numberOfChangesRequired == 2) { bannedPlayer.suspectRating += 45; }
            else if (numberOfChangesRequired == 3) { bannedPlayer.suspectRating += 30; }
        }//Calculates number of char changes to make bannedPlayer persona into the suspects.
        public void compareGameList(Player bannedPlayer) {
            if (bannedPlayer.gameList.Count != 0) {
                double similarity = suspect.playerData.gameList.Intersect(bannedPlayer.gameList).Count();
                similarity = similarity / suspect.playerData.gameList.Count() * 100;
                percentageSimilaritySuspectRatings(bannedPlayer, similarity);
            }
        }
        public void compareRecentGames(Player bannedPlayer) {
            if(bannedPlayer.recentlyPlayed.Count != 0) {
                double similarity = suspect.playerData.recentlyPlayed.Intersect(bannedPlayer.recentlyPlayed).Count();
                similarity = similarity / suspect.playerData.recentlyPlayed.Count() * 100;
                percentageSimilaritySuspectRatings(bannedPlayer, similarity);
            }
        }
        public void comparePrimaryClan(Player bannedPlayer) {
            if(bannedPlayer.primaryClanID == suspect.playerData.primaryClanID) { bannedPlayer.suspectRating += 3; }
        }//Adds to suspect rating of banned player based clan, if the same clan, slightly suspicous
        public void compareAvatar(Player bannedPlayer) {
            if (bannedPlayer.avatar == suspect.playerData.avatar) { bannedPlayer.suspectRating += 5; }
        }//Adds to suspect rating of banned player based on Avatar, if the same, suspicious
        public void compareLocCodes(Player bannedPlayer) {
            if(bannedPlayer.locCountryCode == suspect.playerData.locCountryCode)
            {
                if(bannedPlayer.locStateCode == suspect.playerData.locStateCode){ bannedPlayer.suspectRating += 40; }//If they are in the same country + state
                else { bannedPlayer.suspectRating += 10; }//If they are only in the same country
            }
        }//Adds to suspect rating of banned player based on Account location, if same country - Suspicous, same state - very suspicous
        public void compareRealName(Player bannedPlayer) { 
            if (bannedPlayer.realName == suspect.playerData.realName) { bannedPlayer.suspectRating += 25; }
        }//Adds to suspect rating of banned player based on realname, if the same, very suspicous
        public void startAnalysis()
        {
            foreach(Player bannedPlayer in suspect.suspectList)
            {
                
                Thread thread = new Thread(() =>
                {
                    friendsListAnalysis(bannedPlayer);
                    evaluateUNIX(bannedPlayer);
                    evaluatePlayerLevel(bannedPlayer);
                    compareAvatar(bannedPlayer);
                    comparePrimaryClan(bannedPlayer);
                    compareGameList(bannedPlayer);
                    compareRecentGames(bannedPlayer);
                    comparePersonaName(bannedPlayer);
                });
                threads.Add(thread);
                thread.Start();
                
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
