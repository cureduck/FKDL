using System.Collections.Generic;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Tools;
using UnityEngine;
using Random = System.Random;

namespace Game
{
    public class SecondaryData : SaveData
    {
        public readonly List<string> DiscoveredRelics;
        [JsonIgnore] public Random CurCardRandom;

        [JsonIgnore] public Random CurGameRandom;
        public int InitCardSeed;

        public int InitGameSeed;
        public int InitRelicSeed;
        [JsonIgnore] public Random PotionRandom;
        public int PotionSeed;

        public string[] Prof;
        [JsonIgnore] public Random RelicRandom;

        public int RemoveSkillPoint;

        public Dictionary<Rank, int> SkillPoint;

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


        [JsonConstructor]
        public SecondaryData(RandomState gameRandomState, RandomState cardRandomState, RandomState relicRandomState,
            RandomState potionRandomState) : this()
        {
            GameRandomState = gameRandomState;
            CardRandomState = cardRandomState;
            RelicRandomState = relicRandomState;
            PotionRandomState = potionRandomState;
        }

        [JsonIgnore] public static string _savePath => Application.persistentDataPath + "/SecondarySaveData.json";

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

        public void Save()
        {
            Save(_savePath);
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
            return Load(_savePath);
        }


        private static SecondaryData CreateDefault()
        {
            return new SecondaryData();
        }

        public static SecondaryData GetOrCreate()
        {
            return GetOrCreate(CreateDefault, _savePath);
        }


        public void Init()
        {
#if UNITY_EDITOR
            CurGameRandom = new Random(InitGameSeed);
            CurCardRandom = new Random(InitCardSeed);
            RelicRandom = new Random(InitRelicSeed);
            PotionRandom = new Random(PotionSeed);
#endif

#if !UNITY_EDITOR
            CurGameRandom = new Random((int)Time.time);
            CurCardRandom = new Random((int)Time.time);
            RelicRandom = new Random((int)Time.time);
            PotionRandom = new Random((int)Time.time);

#endif
        }

        public void SetRandom()
        {
            RelicManager.Instance.SetRandom(RelicRandom);
            CrystalManager.Instance.Lib.Random = CurGameRandom;
            SkillManager.Instance.SetRandom(CurCardRandom);
            PotionManager.Instance.SetRandom(PotionRandom);
            BuffManager.Instance.SetRandom(CurGameRandom);
        }


        public static SecondaryData Load(string path)
        {
            return Load<SecondaryData>(path);
        }
    }
}