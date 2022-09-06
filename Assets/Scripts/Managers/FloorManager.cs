using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Managers
{
    public class FloorManager : Singleton<FloorManager>
    {
        public Dictionary<string, Map.Floor> Floors;
        
        
        private static string FloorPath => Path.Combine(Application.dataPath, "Resources", "Floors");
        

        [Button]
        public Map CreateRandomMap()
        {
            Floors = new Dictionary<string, Map.Floor>();

            foreach (var s in Directory.GetDirectories(FloorPath))
            {
                var files = Directory.GetFiles(s);
                var f = File.ReadAllText(files[Random.Range(0, files.Length - 1)]);
                Floors[Path.GetFileName(s)] = JsonConvert.DeserializeObject<Map.Floor>(f, settings: new JsonSerializerSettings
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
    }
}