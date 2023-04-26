using System.Collections.Generic;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
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

        public readonly List<string> DiscoveredRelics;


        [JsonConstructor]
        public SecondaryData()
        {
            SkillPoint = new Dictionary<Rank, int>
            {
                [Rank.Normal] = 0,
                [Rank.Uncommon] = 0,
                [Rank.Rare] = 0
            };
            
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
            
            RelicManager.Instance.SetRandom(RelicRandom);
            CrystalManager.Instance.Lib.Random = CurGameRandom;
            SkillManager.Instance.SetRandom(CurCardRandom);
            PotionManager.Instance.SetRandom(PotionRandom);
            BuffManager.Instance.SetRandom(CurGameRandom);
            
            SkillPoint = new Dictionary<Rank, int>
            {
                [Rank.Normal] = 0,
                [Rank.Uncommon] = 0,
                [Rank.Rare] = 0
            };
        }
        
        public static SecondaryData Load(string path)
        {
            return Load<SecondaryData>(path);
        }
    }
}