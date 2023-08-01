using System;
using System.Collections.Generic;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class Map : SaveData
    {
        public string CurrentFloor;

        public Dictionary<string, Floor> Floors;
        [JsonIgnore] public static string _savePath => Application.persistentDataPath + "/MapData.json";

        public void Save()
        {
            Save(_savePath);
            var s = $"{_savePath}";
        }

        public static Map LoadFromInit()
        {
            return FloorManager.Instance.CreateRandomMap();
        }

        public static Map LoadFromSave()
        {
            return Load(_savePath);
        }

        public void Init()
        {
            foreach (var floor in Floors.Values)
            {
                foreach (var square in floor.Squares)
                {
                    try
                    {
                        square.Init();
                    }
                    catch (Exception e)
                    {
                        if (square is EnemySaveData sq)
                        {
                            Debug.Log($"missing:{sq.Id} floor:{floor.FloorName} crood:{square.Placement}");
                        }
                    }
                }
            }
        }


        public void ReplaceAllEnemy(string targetId, string newId)
        {
            if (EnemyManager.Instance.TryGetEnemy(newId, out var bp))
            {
                foreach (var floor in Floors.Values)
                {
                    foreach (var square in floor.Squares)
                    {
                        if (square is EnemySaveData sq)
                        {
                            if (sq.Id == targetId)
                            {
                                sq.Id = newId;
                                sq.Init();
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("replace enemy failed");
            }
        }


        [Button]
        public static Map Load(string path)
        {
            return Load<Map>(path);
        }

        ~Map()
        {
            Debug.Log("!!!");
        }


        public class Floor
        {
            public string FloorName;
            public LinkedList<MapData> Squares;
        }
    }
}