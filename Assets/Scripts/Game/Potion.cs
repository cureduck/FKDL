using System.Collections.Generic;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Potion : CsvData
    {
        [JsonIgnore] public Texture Icon;
        public Rank Rank { get; set; }
        public float Param1;
        
    }
}