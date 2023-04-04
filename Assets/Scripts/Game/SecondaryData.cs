﻿using System.Collections.Generic;
using Managers;
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
        public int PotionSeed;

        public Random CurGameRandom;
        public Random CurCardRandom;
        public Random RelicRandom;
        public Random PotionRandom;

        public List<string> DiscoveredRelics;


        public SecondaryData()
        {
            DiscoveredRelics = new List<string>();
        }
        
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
            CurGameRandom = new Random(InitGameSeed);
            CurCardRandom = new Random(InitCardSeed);
            RelicRandom = new Random(InitRelicSeed);
            PotionRandom = new Random(PotionSeed);
            
            RelicManager.Instance.Lib.Random = RelicRandom;
            CrystalManager.Instance.Lib.Random = CurGameRandom;
            SkillManager.Instance.Lib.Random = CurCardRandom;
            PotionManager.Instance.Lib.Random = PotionRandom;
        }
        
        public static SecondaryData Load(string path)
        {
            return Load<SecondaryData>(path);
        }
    }
}