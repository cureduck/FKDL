using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using UnityEngine;

namespace Managers
{
    public class Profile : SaveData
    {
        public int CollectedSouls;
        public Dictionary<string, int> GiftsLevel = new Dictionary<string, int>();
        public string[] GiftUnlocks;
        public string[] ProUnlocks;
        public int RecentCollectedSouls;

        private static string _path => Path.Combine(Application.persistentDataPath, "Profile.asset");


        private static Profile NewProfile()
        {
            var giftsLevel = Gift.GiftDictionary.Keys.ToDictionary(key => key, key => 1);

            return new Profile()
            {
                ProUnlocks = new string[] { "MAG", "ALC", "ASS", "KNI", "CUR", "BAR" },
                CollectedSouls = 0,
                GiftsLevel = giftsLevel
            };
        }

        public static Profile GetOrCreate()
        {
            return GetOrCreate(NewProfile, _path);
        }

        public static void Delete()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }
        }

        public void Save()
        {
            Save(_path);
        }
    }
}