using System;
using System.Security.Cryptography;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
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
        public bool NewGame = false;

        public event Action GameLoaded;
        
        [Button]
        public void LoadFromSave()
        {
            NewGame = false;
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
            NewGame = true;
            PlayerData = PlayerData.LoadFromInit();
            Map = Map.LoadFromInit();
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

            while (MapGo.transform.childCount > 0)
            {
                DestroyImmediate(MapGo.transform.GetChild(0).gameObject);
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