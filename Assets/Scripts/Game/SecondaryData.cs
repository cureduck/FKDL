using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

namespace Game
{
    public class SecondaryData : SaveData
    {
        [JsonIgnore] public static string _savePath => Application.persistentDataPath + "/SecondarySaveData.json";

        public Dictionary<Rank, int> SkillPoint;

        public int RemoveSkillPoint;
        
        public string[] Prof;

        public int InitGameSeed;
        public int InitCardSeed;
        public int InitRelicSeed;

        public Random CurGameRandom;
        public Random CurCardRandom;
        public Random RelicRandom;
        
        
        public void Save()
        {
            Save(_savePath);
        }
        
        public static SecondaryData LoadFromInit()
        {
            return null;
        }
        
        public static SecondaryData LoadFromSave()
        {
            return Load(_savePath);
        }

        public void Init()
        {
            
        }
        
        public static SecondaryData Load(string path)
        {
            return Load<SecondaryData>(path);
        }
    }
}