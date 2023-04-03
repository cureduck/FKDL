using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Relic : IRank
    {
        [JsonIgnore] public Texture Icon;
        public string Id;
        public string Description;

        [JsonIgnore] public Dictionary<Timing, MethodInfo> Fs;
        public Rank Rank { get; set; }
    }
}