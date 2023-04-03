using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Cysharp.Threading.Tasks;
using Game;
using Sirenix.OdinInspector;
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
        
        public GameObject Prefab;

        public GameObject MapGo;

        public Square Focus;

        public bool InBattle => (Focus?.Data is EnemySaveData e) && (e.IsAlive);

        public Dictionary<string, Color> SquareColors;

        public event Action GameLoaded;
        public event Action<Square> FocusChanged;
        
        public void BroadcastSquare(Square square)
        {
            FocusChanged?.Invoke(square);
        }
        
        
        
        private void Start()
        {
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(this);
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
            var offers = new Offer[3];
            var skills = SkillManager.Instance.Roll(rank, 3);
            for (int i = 0; i < 3; i++)
            {
                offers[i] = new Offer()
                {
                    Id = skills[i],
                    Kind = Offer.OfferKind.Skill
                };
            }
            WindowManager.Instance.OffersWindow.Load(offers);
        }


        private void SetSeed(int CardSeed, int GameSeed, int RelicSeed)
        {
            SecondaryData.CurCardRandom = new Random(CardSeed);
            SecondaryData.CurGameRandom = new Random(GameSeed);
            SecondaryData.RelicRandom = new Random(RelicSeed);

            RelicManager.Instance.Lib.Random = SecondaryData.RelicRandom;
            CrystalManager.Instance.Lib.Random = SecondaryData.CurGameRandom;
            SkillManager.Instance.Lib.Random = SecondaryData.CurCardRandom;
        }
        


        [Button]
        public void LoadFromSave()
        {
            try
            {
                PlayerData = PlayerData.LoadFromSave();
                SecondaryData = SecondaryData.LoadFromSave();
                SetSeed(SecondaryData.InitCardSeed, SecondaryData.InitGameSeed, SecondaryData.InitRelicSeed);
                Map = Map.LoadFromSave();
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
            SetSeed(SecondaryData.InitCardSeed, SecondaryData.InitGameSeed, SecondaryData.InitRelicSeed);
            PlayerData = PlayerData.LoadFromInit();
            PlayerData.Gain(0);
            Map = Map.LoadFromInit();
            Map.Init();
            LoadMap();
            GameLoaded?.Invoke();
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
            squares.Clear();
            foreach (Transform trans in MapGo.transform)
            {
                Destroy(trans.gameObject);
            }
            

            foreach (var square in floor.Squares)
            {
                Square curObject = CreateSquare(square);
                if (curObject != null) 
                {
                    squares.Add(curObject);
                }
            }
        }

        public Square GetByData(MapData mapData) 
        {
            for (int i = 0; i < squares.Count; i++)
            {
                if (squares[i].Data == mapData) 
                {
                    return squares[i];
                }
            }
            return null;
        }

        
        [Button]
        private Square CreateSquare(MapData data)
        {
            if (data == null) return null;
            
            var go = Instantiate(Prefab, MapGo.transform);
            try
            {
                var sq = go.GetComponent<Square>();
                sq.Data = data;
                return sq;
            }
            catch (Exception e)
            {
                DestroyImmediate(go);
                Console.WriteLine(e);
                throw;
            }
        }
    }
}