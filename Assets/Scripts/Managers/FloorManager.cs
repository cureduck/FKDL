using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using Random = UnityEngine.Random;

namespace Managers
{
    public class FloorManager : Singleton<FloorManager>
    {
        private static string FloorPath => Path.Combine(Application.streamingAssetsPath, "Floors");

        [Button]
        public Map CreateRandomMap()
        {
            var Floors = new Dictionary<string, Map.Floor>();

            foreach (var s in Directory.GetDirectories(FloorPath))
            {
                var files = Directory.GetFiles(s).ToList().FindAll((s1 => s1.EndsWith(".json"))).ToArray();

                var chosen = GameDataManager.Instance.SecondaryData.CurGameRandom.Next(0, files.Length - 1);

                Debug.Log($"Loading {s} {chosen}");

                var f = File.ReadAllText(files[chosen]);
                Floors[Path.GetFileName(s)] = JsonConvert.DeserializeObject<Map.Floor>(f,
                    settings: new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
            }

            return new Map
            {
                Floors = Floors,
                CurrentFloor = "1"
            };
        }


        [Button]
        public void LoadAllFloors()
        {
            foreach (var s in Directory.GetDirectories(FloorPath))
            {
                var files = Directory.GetFiles(s).ToList().FindAll((s1 => s1.EndsWith(".json"))).ToArray();

                foreach (var file in files)
                {
                    try
                    {
                        var floor = JsonConvert.DeserializeObject<Map.Floor>(File.ReadAllText(file),
                            settings: new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.All
                            });
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"Loading {file} failed");
                    }
                }
            }
        }


        [Button("Create Floor")]
        public static void CreateFloor(Map.Floor floor)
        {
            var f = JsonConvert.SerializeObject(floor, settings: new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                //Formatting = Formatting.Indented
            });

            if (!Directory.Exists(Path.Combine(FloorPath, floor.FloorName)))
            {
                Directory.CreateDirectory(Path.Combine(FloorPath, floor.FloorName));
            }

            var count = Directory.GetFiles(Path.Combine(FloorPath, floor.FloorName))
                .Count(n => n.EndsWith(".json"));

            File.WriteAllText(Path.Combine(FloorPath, floor.FloorName, count + ".json"), f);
        }


        [Button]
        public Map.Floor SaveCurrentFloor(string floorName)
        {
            var f = new Map.Floor
            {
                Squares = new LinkedList<MapData>(),
                FloorName = floorName
            };
            foreach (Transform go in GameManager.Instance.MapGo.transform)
            {
                f.Squares.AddLast(go.GetComponent<Square>().Data);
            }

            return f;
        }
    }
}