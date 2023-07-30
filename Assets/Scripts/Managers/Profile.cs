using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using UnityEngine;

namespace Managers
{
    public class Profile : SaveData
    {
        public static string[] CurAllPorf = new string[] { "MAG", "ALC", "ASS", "KNI", "CUR", "BAR" };

        public AchievementsMonitor AchievementsMonitor;
        public int CollectedSouls;
        public Dictionary<string, int> GiftsLevel = new Dictionary<string, int>();
        public string[] GiftUnlocks;
        public string[] ProUnlocks;

        public int RecentCollectedSouls;
        public HashSet<string> UnlockedCards;

        private static string _path => Path.Combine(Application.streamingAssetsPath, "Profile.asset");


        private static Profile CreateNewProfile()
        {
            var giftsLevel = Gift.GiftDictionary.Keys.ToDictionary(key => key, key => 1);

            return new Profile()
            {
                ProUnlocks = new string[] { "MAG", "ALC", "ASS", /*"KNI", "CUR", "BAR"*/ },
                CollectedSouls = 1000,
                GiftsLevel = giftsLevel,
                AchievementsMonitor = Game.AchievementsMonitor.GetDefault(),
                UnlockedCards = new HashSet<string>()
            };
        }

        public static Profile GetOrCreate()
        {
            return GetOrCreate(CreateNewProfile, _path);
        }

        public static void Delete()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }
        }

        public int GetUnlockProfCost(string prof)
        {
            return ProUnlocks.Length * 30 - 60;
        }

        public void Save()
        {
            Save(_path);
        }
    }
}