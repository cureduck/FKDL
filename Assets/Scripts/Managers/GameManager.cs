using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using I2.Loc;
using Sirenix.OdinInspector;
using Tools;
using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(LocalizationParamsManager))]
    public class GameManager : Singleton<GameManager>
    {
        public LocalizationParamsManager GlobalLocalizationParamsManager;

        public Square Prefab;

        public Transform MapGo;

        public Square Focus;

        private ObjectPool<Square> _pool;

        public Profile Profile;

        public Dictionary<string, Color> SquareColors;

        private List<Square> squares = new List<Square>();
        public string CurFloor => $"{Map.CurrentFloor}";

        public PlayerData PlayerData
        {
            get => GameDataManager.Instance.PlayerData;
            set => GameDataManager.Instance.PlayerData = value;
        }

        public Map Map
        {
            get => GameDataManager.Instance.Map;
            private set => GameDataManager.Instance.Map = value;
        }

        public SecondaryData SecondaryData
        {
            get => GameDataManager.Instance.SecondaryData;
            private set => GameDataManager.Instance.SecondaryData = value;
        }


        public bool InBattle => (Focus?.Data is EnemySaveData e) && (e.IsAlive);

        private void Start()
        {
            GlobalLocalizationParamsManager = GetComponent<LocalizationParamsManager>();
#if UNITY_EDITOR
            GetLocalization();
#endif
            Profile = ProfileManager.Instance.Profile;
            _pool = new ObjectPool<Square>(CreateSquare);

            if (Application.isPlaying)
            {
                if (SceneSwitchManager.Instance.NewGame)
                {
                    LoadFromInit();
                }
                else
                {
                    LoadFromSave();
                }
            }
        }

        public void SkipReward(out SkipInfo info)
        {
            info = new SkipInfo(10);
            PlayerData.Gain(10);
        }


        public Square FindStartSquare()
        {
            return squares.Find(square => square.Data is StartSaveData);
        }

        private static void GetLocalization()
        {
            var localizationFilePath = "Assets/Localization";
            try
            {
                var localizationFiles = Directory.GetFiles(localizationFilePath);
                foreach (var fileName in localizationFiles)
                {
                    if (fileName.EndsWith(".csv"))
                    {
                        var f = File.ReadAllText(fileName);
                        f = f.Replace("\r\n", "\n");
                        LocalizationManager.Sources[0].Import_CSV("", f, eSpreadsheetUpdateMode.Merge);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public event Action GameLoaded;
        public event Action<string> Marched;
        public event Action<Square> FocusChanged;

        public void BroadcastSquareChanged(Square square)
        {
            FocusChanged?.Invoke(square);
        }

        private Square CreateSquare()
        {
            return Instantiate(Prefab, MapGo);
        }

        public void RollForSkill(int rank)
        {
            RollForSkill((Rank)rank);
        }

        public void RollForPotion(int rank)
        {
            var potions = PotionManager.Instance.GenerateT((Rank)rank, PlayerData.LuckyChance, 3);

            var offers = potions.Select((s => new Offer(s)));

            WindowManager.Instance.OffersWindow.Load(offers);
        }


        public void Attack()
        {
            if (PlayerData.Enemy != null)
            {
                PlayerData.OnReact();
            }
        }

        public void RollForSkill(Rank rank)
        {
            var skills = SkillManager.Instance.GenerateT(rank, PlayerData.LuckyChance, 3);

            var offers = skills.Select((s => new Offer(s)));

            WindowManager.Instance.OffersWindow.Load(offers);
        }

        public void RollForRelic(int rank)
        {
            RollForRelic((Rank)rank);
        }


        public void RollForRelic(Rank rank)
        {
            var skills = RelicManager.Instance.RollT(rank, 3);

            var offers = skills.Select((s => new Offer(s)));

            WindowManager.Instance.OffersWindow.Load(offers);
        }


        [Button]
        public void LoadFromSave()
        {
            try
            {
                PlayerData = PlayerData.LoadFromSave();
                SecondaryData = SecondaryData.LoadFromSave();
                SecondaryData.SetRandom();
                Map = Map.LoadFromSave();
                PlayerData.BroadCastUpdated();
                LoadMap();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            GameLoaded?.Invoke();
            //GC.Collect();
        }

        [Button]
        public void LoadFromInit()
        {
            SecondaryData.Init();
            SecondaryData.SetRandom();
            PlayerData = PlayerData.LoadFromInit();
            PlayerData.BroadCastUpdated();
            Map = Map.LoadFromInit();
            Map.Init();
            LoadMap();
            GameLoaded?.Invoke();
            //GameManager.Instance.PlayerData.Gain(10000);
            //GC.Collect();
        }

        [Button]
        public void Save()
        {
            PlayerData.Save();
            Map.Save();
            //SecondaryData.CurCardSeed = CardRand.
            SecondaryData.Save();
        }


        private void LoadMap()
        {
            GameManager.Instance.Focus = null;
            LoadFloor(Map.Floors[Map.CurrentFloor]);
        }

        public void LoadFloor(Map.Floor floor)
        {
            Marched?.Invoke(floor.FloorName);
            foreach (var square in squares)
            {
                square.UnbindCurrent();
                square.gameObject.SetActive(false);
                _pool.Return(square);
            }

            squares.Clear();
            /*foreach (Transform trans in MapGo)
            {
                _pool.Return(trans.gameObject.GetComponent<>());
            }*/


            foreach (var square in floor.Squares)
            {
                Square curObject = CreateSquare(square);
                if (curObject != null)
                {
                    squares.Add(curObject);
                }
            }
        }


        [Button]
        private Square CreateSquare(MapData data)
        {
            if (data == null) return null;

            //var sq = Instantiate(Prefab, MapGo);
            var sq = _pool.Get();
            sq.Reload(data);

            try
            {
                sq.Data = data;
                return sq;
            }
            catch (Exception e)
            {
                DestroyImmediate(sq.gameObject);
                Console.WriteLine(e);
                throw;
            }
        }
    }
}