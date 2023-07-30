using System.Collections.Generic;
using System.IO;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Tools;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Game
{
    public class SecondaryData : SaveData
    {
        public readonly List<string> DiscoveredRelics;
        public int BreakoutPoint;
        [JsonIgnore] public Random CurCardRandom;

        [JsonIgnore] public Random CurGameRandom;

        public Gift[] Gifts;

        public int InitGameSeed;
        public HashSet<string> Killed = new HashSet<string>();
        public string[] Nightmares;

        [JsonIgnore] public Random PotionRandom;

        [FormerlySerializedAs("Prof")] public string[] Profs;


        [JsonIgnore] public Random RelicRandom;

        public int RemoveSkillPoint;

        public int SkillPoint = 100;

        public Dictionary<string, bool> Switches = new Dictionary<string, bool>();

        public SecondaryData()
        {
            SkillPoint = 0;
            DiscoveredRelics = new List<string>();
        }


        [JsonConstructor]
        public SecondaryData(RandomState gameRandomState, RandomState cardRandomState, RandomState relicRandomState,
            RandomState potionRandomState) : this()
        {
            GameRandomState = gameRandomState;
            CardRandomState = cardRandomState;
            RelicRandomState = relicRandomState;
            PotionRandomState = potionRandomState;
        }

        public SecondaryData(int initGameSeed) : this()
        {
            InitGameSeed = initGameSeed;
            Init();
        }

        [JsonIgnore] public float NightmareBonusRate => Game.Nightmares.GetAllNightmareBonus(Nightmares);


        [JsonIgnore] private static string SavePath => Application.persistentDataPath + "/SecondarySaveData.json";

        public RandomState GameRandomState
        {
            get => CurGameRandom.Save();
            set => CurGameRandom = value.Restore();
        }

        public RandomState CardRandomState
        {
            get => CurCardRandom.Save();
            set => CurCardRandom = value.Restore();
        }

        public RandomState RelicRandomState
        {
            get => RelicRandom.Save();
            set => RelicRandom = value.Restore();
        }

        public RandomState PotionRandomState
        {
            get => PotionRandom.Save();
            set => PotionRandom = value.Restore();
        }


        public double NextDouble()
        {
            return CurGameRandom.NextDouble();
        }

        public int NextInt(int min, int max)
        {
            return CurGameRandom.Next(min, max);
        }

        internal static void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }
        }

        public void Save()
        {
            Save(SavePath);
        }

        [Button]
        public void SaveToInit()
        {
            var path = Application.streamingAssetsPath + "/SecondarySaveData.json";
            Save(path);
        }


        public static SecondaryData LoadFromInit()
        {
            return null;
        }

        public static SecondaryData LoadFromSave()
        {
            return Load(SavePath);
        }


        private static SecondaryData CreateDefault()
        {
            var seed = UnityEngine.Random.Range(0, 100000);
            return new SecondaryData(seed);
        }

        public static SecondaryData GetOrCreate()
        {
            return GetOrCreate(CreateDefault, SavePath);
        }


        public void Init()
        {
            CurGameRandom = new Random(InitGameSeed);
            CurCardRandom = new Random(CurGameRandom.Next());
            RelicRandom = new Random(CurGameRandom.Next());
            PotionRandom = new Random(CurGameRandom.Next());
        }

        public void SetRandom()
        {
            RelicManager.Instance.SetRandom(RelicRandom);
            CrystalManager.Instance.Lib.Random = CurGameRandom;
            SkillManager.Instance.SetRandom(CurCardRandom);
            PotionManager.Instance.SetRandom(PotionRandom);
            BuffManager.Instance.SetRandom(CurGameRandom);
        }


        private static SecondaryData Load(string path)
        {
            return Load<SecondaryData>(path);
        }
    }
}