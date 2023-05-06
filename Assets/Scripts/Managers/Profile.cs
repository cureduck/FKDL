using System;
using System.IO;
using Game;
using UnityEditor;
using UnityEngine;

namespace Managers
{
    public class Profile : SaveData
    {
        public string[] Unlocks;
        public int Progress;


        public static Profile NewProfile()
        {
            return new Profile()
            {
                Unlocks = new string[]{"MAG", "ALC", "ASS"},
                Progress = 0
            };
        }
        
        private static string _path => Path.Combine(Application.persistentDataPath, "Profile.asset");
        public static Profile GetOrCreate()
        {
            Profile so;
            if (File.Exists(_path))
            {
                try
                {
                    so = Load<Profile>(_path);
                }
                catch (Exception e)
                {
                    File.Delete(_path);
                    Debug.LogError(e);
                    throw;
                }
            }
            else
            {
                so = NewProfile();
                so.Save(_path);
            }

            return so;
        }

        public void Save()
        {
            Save(_path);
        }
    }
}