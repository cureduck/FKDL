using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Potion : IRank
    {
        [JsonIgnore] public Texture Icon;
        public string Id;
        public Rank Rank { get; set; }
        public float Param1;
        
        public Dictionary<Timing, MethodInfo> Fs;
    }
}