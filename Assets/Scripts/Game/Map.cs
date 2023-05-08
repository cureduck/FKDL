using System;
using System.Collections.Generic;
using System.IO;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class Map : SaveData
    {
        [JsonIgnore] public static string _savePath => Application.persistentDataPath + "/MapData.json";

        public string CurrentFloor;

        public Dictionary<string, Floor> Floors;


        public class Floor
        {
            public LinkedList<MapData> Squares;
            public string FloorName;
        }

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


        [Button]
        public static Map Load(string path)
        {
            return Load<Map>(path);
        }

        ~Map()
        {
            Debug.Log("!!!");
        }
    }
}