using System;
using System.Security.Cryptography;
using Cysharp.Threading.Tasks;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Managers
{
    [ExecuteAlways]
    public class GameManager : Singleton<GameManager>
    {
        public PlayerData PlayerData;
        public Map Map;

        public GameObject Prefab;

        public GameObject MapGo;

        public Random Random;
        public Square Focus;

        public event Action GameLoaded;


        public void RollForSkill(int rank)
        {
            RollForSkill((Rank)rank);
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
        
        
        
        [Button]
        public void LoadFromSave()
        {
            try
            {
                PlayerData = PlayerData.LoadFromSave();
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
            PlayerData = PlayerData.LoadFromInit();
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
        }
        
        
        private void LoadMap()
        {
            LoadFloor(Map.Floors[Map.CurrentFloor]);
        }

        public void LoadFloor(Map.Floor floor)
        {
            foreach (Transform trans in MapGo.transform)
            {
                Destroy(trans.gameObject);
            }
            

            foreach (var square in floor.Squares)
            {
                CreateSquare(square);
            }
        }
        
        [Button]
        private void CreateSquare(MapData data)
        {
            if (data == null) return;
            
            var go = Instantiate(Prefab, MapGo.transform);
            try
            {
                var sq = go.GetComponent<Square>();
                sq.Data = data;
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