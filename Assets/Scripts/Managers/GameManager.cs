﻿using System;
using System.Collections.Generic;
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

        public bool InBattle => Focus?.Data is EnemySaveData;

        public Dictionary<string, Color> SquareColors;

        public event Action GameLoaded;


        private void Start()
        {
            DontDestroyOnLoad(this);
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
        
        
        
        [Button]
        public void LoadFromSave()
        {
            try
            {
                PlayerData = PlayerData.LoadFromSave();
                PlayerData.Gain(0);
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