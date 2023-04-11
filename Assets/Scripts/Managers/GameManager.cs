using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        public void BroadcastSquareChanged(Square square)
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
            var skills = SkillManager.Instance.RollT(rank, 3);

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