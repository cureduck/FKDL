﻿using System.IO;
using Game;
using UnityEngine;

namespace Managers
{
    public class Profile : SaveData
    {
        public int Progress;
        public string[] Unlocks;

        private static string _path => Path.Combine(Application.persistentDataPath, "Profile.asset");


        private static Profile NewProfile()
        {
            return new Profile()
            {
                Unlocks = new string[] { "MAG", "ALC", "ASS" },
                Progress = 0
            };
        }

        public static Profile GetOrCreate()
        {
            return GetOrCreate(NewProfile, _path);
        }

        public void Save()
        {
            Save(_path);
        }
    }
}