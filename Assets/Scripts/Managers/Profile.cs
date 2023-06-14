using System.IO;
using Game;
using UnityEngine;

namespace Managers
{
    public class Profile : SaveData
    {
        public int CollectedSouls;
        public string[] GiftUnlocks;
        public string[] ProUnlocks;
        public int RecentCollectedSouls;

        private static string _path => Path.Combine(Application.persistentDataPath, "Profile.asset");


        private static Profile NewProfile()
        {
            return new Profile()
            {
                ProUnlocks = new string[] { "MAG", "ALC", "ASS", "KNI", "CUR", "BAR" },
                CollectedSouls = 0
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