using System;
using System.Security.Cryptography;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    [ExecuteAlways]
    public class GameManager : Singleton<GameManager>
    {
        public PlayerData PlayerData;
        public Map Map;

        public GameObject Prefab;

        public GameObject MapGo;


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
        }
        
        [Button]
        public void LoadFromInit()
        {
            PlayerData = PlayerData.LoadFromInit();
            Map = Map.LoadFromInit();
            LoadMap();
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
            foreach (Transform child in MapGo.transform)
            {
                DestroyImmediate(child.gameObject);
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
                switch (data)
                {
                    case EnemySaveData d1:
                        var sq = go.AddComponent<EnemySquare>();
                        sq.Data = d1;
                        break;
                }
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