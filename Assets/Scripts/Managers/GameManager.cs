using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Cysharp.Threading.Tasks;
using Game;
using I2.Loc;
using Sirenix.OdinInspector;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

namespace Managers
{
    [ExecuteAlways]
    public class GameManager : Singleton<GameManager>
    {
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


        public void GetLocalization()
        {
            var f = File.ReadAllText("Assets/PythonScripts/relics_loc.csv");
            LocalizationManager.Sources[0].Import_CSV("", f, eSpreadsheetUpdateMode.AddNewTerms);
            f = File.ReadAllText("Assets/PythonScripts/skills_loc.csv");
            LocalizationManager.Sources[0].Import_CSV("", f, eSpreadsheetUpdateMode.AddNewTerms);
        }
        
        
        public Square Prefab;

        public Transform MapGo;

        public Square Focus;

        public bool InBattle => (Focus?.Data is EnemySaveData e) && (e.IsAlive);

        public Dictionary<string, Color> SquareColors;

        public event Action GameLoaded;
        public event Action<Square> FocusChanged;
        
        public void BroadcastSquareChanged(Square square)
        {
            FocusChanged?.Invoke(square);
        }

        private Square CreateSquare()
        {
            return Instantiate(Prefab, MapGo);
        }
        
        private ObjectPool<Square> _pool;
        
        private void Start()
        {
            GetLocalization();
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
        
        public void RollForSkill(int rank)
        {
            RollForSkill((Rank)rank);
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
        
        public void RollForRelic(Rank rank)
        {
            var skills = RelicManager.Instance.GenerateT(rank, PlayerData.LuckyChance, 3);

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
                SecondaryData.Init();
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
            LoadFloor(Map.Floors[Map.CurrentFloor]);
        }

        private List<Square> squares = new List<Square>();
        
        public void LoadFloor(Map.Floor floor)
        {
            foreach (var square in squares)
            {
                square.gameObject.SetActive(false);
                square.UnbindCurrent();
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